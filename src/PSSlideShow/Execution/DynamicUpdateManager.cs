using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using PSSlideShow.Model;

namespace PSSlideShow.Execution {
    /// <summary>
    /// Runs dynamic widget updates in dedicated background runspaces and publishes completed snapshots
    /// for the host thread to turn into views.
    /// </summary>
    public sealed class DynamicUpdateManager : IDisposable {
        private readonly RunspaceManager _runspaces;
        private readonly ConcurrentDictionary<DynamicWorker, DynamicUpdateResult> _completed = new();
        private readonly object _sync = new object();
        private readonly Dictionary<DynamicWidget, DynamicWorker> _workers = new();

        public DynamicUpdateManager(RunspaceManager runspaces) {
            _runspaces = runspaces ?? throw new ArgumentNullException(nameof(runspaces));
        }

        /// <summary>
        /// Replaces the active workers and performs each new widget's first update before returning.
        /// </summary>
        public void Activate(Slide slide) {
            if (slide == null) throw new ArgumentNullException(nameof(slide));

            StopActive();

            var workers = new List<DynamicWorker>();
            foreach (var widget in DynamicCollector.Collect(slide)) {
                var worker = new DynamicWorker(
                    widget,
                    _runspaces.GetDynamicRunspace(widget),
                    _completed);

                lock (_sync) {
                    _workers[widget] = worker;
                }

                workers.Add(worker);
            }

            foreach (var worker in workers) {
                worker.Start();
            }

            foreach (var worker in workers) {
                Apply(worker, worker.WaitForInitial());
            }
        }

        /// <summary>Applies completed background results on the host thread.</summary>
        public bool ProcessCompleted() {
            var changed = false;
            foreach (var pair in _completed.ToArray()) {
                if (_completed.TryRemove(pair.Key, out var result) && IsCurrent(pair.Key)) {
                    Apply(pair.Key, result);
                    changed = true;
                }
            }

            return changed;
        }

        public void Dispose() => StopActive();

        private void Apply(DynamicWorker worker, DynamicUpdateResult result) {
            if (result.Error == null) {
                worker.Widget.ApplySnapshot(result.Snapshot, result.CompletedAt);
            }
            else {
                worker.Widget.ApplyError(result.Error);
            }
        }

        private bool IsCurrent(DynamicWorker worker) {
            lock (_sync) {
                return _workers.TryGetValue(worker.Widget, out var current) && ReferenceEquals(current, worker);
            }
        }

        private void StopActive() {
            DynamicWorker[] workers;
            lock (_sync) {
                workers = _workers.Values.ToArray();
                _workers.Clear();
            }

            foreach (var worker in workers) {
                worker.Stop();
                _runspaces.ReleaseDynamicRunspace(worker.Widget);
            }

            _completed.Clear();
        }

        private sealed class DynamicWorker {
            private readonly ConcurrentDictionary<DynamicWorker, DynamicUpdateResult> _completed;
            private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
            private readonly TaskCompletionSource<DynamicUpdateResult> _initial =
                new TaskCompletionSource<DynamicUpdateResult>();
            private readonly object _sync = new object();
            private readonly Runspace _runspace;
            private Task? _task;
            private PowerShell? _current;

            public DynamicWorker(
                DynamicWidget widget,
                Runspace runspace,
                ConcurrentDictionary<DynamicWorker, DynamicUpdateResult> completed) {
                Widget = widget;
                _runspace = runspace;
                _completed = completed;
            }

            public DynamicWidget Widget { get; }

            public void Start() {
                _task = Task.Factory.StartNew(
                    Run,
                    CancellationToken.None,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
            }

            public DynamicUpdateResult WaitForInitial() => _initial.Task.GetAwaiter().GetResult();

            public void Stop() {
                _cancellation.Cancel();

                PowerShell? current;
                lock (_sync) {
                    current = _current;
                }

                try {
                    current?.Stop();
                }
                catch {
                }

                try {
                    _task?.Wait();
                }
                catch (AggregateException) {
                }
                finally {
                    _cancellation.Dispose();
                }
            }

            private void Run() {
                try {
                    var first = ExecuteUpdate();
                    if (!_initial.TrySetResult(first)) {
                        _completed[this] = first;
                    }

                    while (!_cancellation.IsCancellationRequested && WaitInterval()) {
                        var result = ExecuteUpdate();
                        if (!_cancellation.IsCancellationRequested) {
                            _completed[this] = result;
                        }
                    }
                }
                catch (Exception ex) {
                    var result = DynamicUpdateResult.Failure(ex.Message);
                    if (!_initial.TrySetResult(result)) {
                        _completed[this] = result;
                    }
                }
            }

            private bool WaitInterval() {
                try {
                    Task.Delay(Widget.UpdateInterval, _cancellation.Token).GetAwaiter().GetResult();
                    return true;
                }
                catch (OperationCanceledException) {
                    return false;
                }
            }

            private DynamicUpdateResult ExecuteUpdate() {
                using var powerShell = PowerShell.Create();
                powerShell.Runspace = _runspace;
                powerShell.AddScript(Widget.UpdateScript.ToString());

                lock (_sync) {
                    _current = powerShell;
                }

                try {
                    var results = powerShell.Invoke();
                    if (powerShell.HadErrors && powerShell.Streams.Error.Count > 0) {
                        var error = string.Join(
                            Environment.NewLine,
                            powerShell.Streams.Error.Select(item => item.ToString()));
                        return DynamicUpdateResult.Failure(error);
                    }

                    return DynamicUpdateResult.Success(Normalize(results));
                }
                catch (RuntimeException ex) {
                    return DynamicUpdateResult.Failure(ex.Message);
                }
                catch (Exception ex) {
                    return DynamicUpdateResult.Failure(ex.Message);
                }
                finally {
                    lock (_sync) {
                        if (ReferenceEquals(_current, powerShell)) {
                            _current = null;
                        }
                    }
                }
            }

            private static object? Normalize(IList<PSObject> results) {
                if (results.Count == 0) {
                    return null;
                }

                if (results.Count == 1) {
                    return NormalizeOne(results[0]);
                }

                return results.Select(NormalizeOne).ToArray();
            }

            private static object? NormalizeOne(PSObject? result) {
                if (result == null) {
                    return null;
                }

                return result.BaseObject is PSCustomObject ? result : result.BaseObject;
            }
        }

        private sealed class DynamicUpdateResult {
            private DynamicUpdateResult(object? snapshot, string? error, DateTimeOffset completedAt) {
                Snapshot = snapshot;
                Error = error;
                CompletedAt = completedAt;
            }

            public object? Snapshot { get; }
            public string? Error { get; }
            public DateTimeOffset CompletedAt { get; }

            public static DynamicUpdateResult Success(object? snapshot) =>
                new DynamicUpdateResult(snapshot, null, DateTimeOffset.UtcNow);

            public static DynamicUpdateResult Failure(string error) =>
                new DynamicUpdateResult(null, error, DateTimeOffset.UtcNow);
        }
    }
}