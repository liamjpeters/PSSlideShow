using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Linq;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PSSlideShow.Model {
    /// <summary>
    /// A widget whose data is refreshed in the background and whose view is rebuilt on the host thread.
    /// </summary>
    public sealed class DynamicWidget : SlideWidget {
        private IReadOnlyList<ISlideWidget> _view = Array.Empty<ISlideWidget>();
        private bool _hasSnapshot;
        private string? _lastError;

        public DynamicWidget(TimeSpan interval, ScriptBlock update, ScriptBlock view) {
            if (interval <= TimeSpan.Zero) {
                throw new ArgumentOutOfRangeException(nameof(interval), "The update interval must be positive.");
            }

            UpdateInterval = interval;
            UpdateScript = update ?? throw new ArgumentNullException(nameof(update));
            ViewScript = view ?? throw new ArgumentNullException(nameof(view));
        }

        public TimeSpan UpdateInterval { get; }
        public ScriptBlock UpdateScript { get; }
        public ScriptBlock ViewScript { get; }
        public object? Snapshot { get; private set; }
        public DateTimeOffset? LastUpdated { get; private set; }
        public string? LastError => _lastError;
        public bool HasSnapshot => _hasSnapshot;
        public override bool IsRunnable => false;

        public override IRenderable Render(WidgetRenderContext context) {
            if (!_hasSnapshot && _lastError == null) {
                return new Markup("Waiting for first update...", context.Theme.Chrome);
            }

            if (_view.Count == 0) {
                var message = _lastError == null
                    ? string.Empty
                    : "Dynamic update failed: " + Markup.Escape(_lastError);
                return new Markup(message, _lastError == null ? context.Theme.Content : context.Theme.Error);
            }

            if (_view.Count == 1) {
                return _view[0].Render(context);
            }

            return new Rows(_view.Select(widget => widget.Render(context)).ToArray());
        }

        internal void ApplySnapshot(object? snapshot, DateTimeOffset updated) {
            try {
                var widgets = InvokeView(snapshot);
                Snapshot = snapshot;
                _view = widgets;
                _hasSnapshot = true;
                LastUpdated = updated;
                _lastError = null;
            }
            catch (Exception ex) {
                _lastError = ex.Message;
            }
        }

        internal void ApplyError(string error) {
            _lastError = string.IsNullOrWhiteSpace(error) ? "The update failed." : error;
        }

        private IReadOnlyList<ISlideWidget> InvokeView(object? snapshot) {
            var variables = new List<PSVariable>
            {
                new PSVariable("_", snapshot),
                new PSVariable("PSItem", snapshot)
            };

            var results = ViewScript.InvokeWithContext(null, variables, snapshot);
            var widgets = new List<ISlideWidget>();
            foreach (var result in results) {
                if (result?.BaseObject is ISlideWidget widget) {
                    widgets.Add(widget);
                }
            }

            return widgets;
        }
    }
}