using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using PSSlideShow.Model;

namespace PSSlideShow.Execution {
    /// <summary>
    /// Creates, caches, and disposes runspaces according to each slide's <see cref="CodeScope"/>, and
    /// resolves which runspace a given code block should execute in.
    /// </summary>
    public sealed class RunspaceManager : IDisposable {
        private static readonly object ShowKey = new object();
        private readonly object _sync = new object();
        private readonly Dictionary<object, Runspace> _runspaces = new();
        private readonly Dictionary<DynamicWidget, Runspace> _dynamicRunspaces = new();

        /// <summary>Resolves (creating if needed) the runspace for a block on a slide.</summary>
        public Runspace GetRunspace(Slide slide, CodeWidget block) {
            lock (_sync) {
                var key = KeyFor(slide, block);
                if (!_runspaces.TryGetValue(key, out var runspace) ||
                    runspace.RunspaceStateInfo.State != RunspaceState.Opened) {
                    runspace = Create();
                    _runspaces[key] = runspace;
                }

                return runspace;
            }
        }

        /// <summary>Gets the dedicated runspace used by a dynamic widget's update worker.</summary>
        public Runspace GetDynamicRunspace(DynamicWidget widget) {
            lock (_sync) {
                if (!_dynamicRunspaces.TryGetValue(widget, out var runspace) ||
                    runspace.RunspaceStateInfo.State != RunspaceState.Opened) {
                    runspace = Create();
                    _dynamicRunspaces[widget] = runspace;
                }

                return runspace;
            }
        }

        /// <summary>Releases a dynamic widget's dedicated runspace.</summary>
        public void ReleaseDynamicRunspace(DynamicWidget widget) {
            Runspace? runspace = null;
            lock (_sync) {
                if (_dynamicRunspaces.TryGetValue(widget, out runspace)) {
                    _dynamicRunspaces.Remove(widget);
                }
            }

            runspace?.Dispose();
        }

        /// <summary>Disposes the runspace(s) backing the current slide so it can be re-run cleanly.</summary>
        public void ResetSlide(Slide slide) {
            var blocks = new HashSet<CodeWidget>(RunnableCollector.Collect(slide));
            List<Runspace> toDispose;
            lock (_sync) {
                var toRemove = _runspaces.Keys.Where(key =>
                    ReferenceEquals(key, slide) ||
                    (key is CodeWidget widget && blocks.Contains(widget)) ||
                    (ReferenceEquals(key, ShowKey) && slide.CodeScope == CodeScope.Show)).ToList();

                toDispose = new List<Runspace>(toRemove.Count);
                foreach (var key in toRemove) {
                    toDispose.Add(_runspaces[key]);
                    _runspaces.Remove(key);
                }
            }

            foreach (var runspace in toDispose) {
                runspace.Dispose();
            }
        }

        public void Dispose() {
            List<Runspace> toDispose;
            lock (_sync) {
                toDispose = _runspaces.Values.Concat(_dynamicRunspaces.Values).ToList();
                _runspaces.Clear();
                _dynamicRunspaces.Clear();
            }

            foreach (var runspace in toDispose) {
                runspace.Dispose();
            }
        }

        private object KeyFor(Slide slide, CodeWidget block) => slide.CodeScope switch {
            CodeScope.Show => ShowKey,
            CodeScope.Block => block,
            _ => slide
        };

        private static Runspace Create() {
            var runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();

            using var powerShell = PowerShell.Create();
            powerShell.Runspace = runspace;
            powerShell.AddScript("if ($null -ne $PSStyle) { $PSStyle.OutputRendering = 'Ansi' }");
            powerShell.Invoke();

            return runspace;
        }
    }
}
