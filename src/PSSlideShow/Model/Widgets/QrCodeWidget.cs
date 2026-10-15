using System;
using System.Collections.Generic;
using Spectre.Console;
using Spectre.Console.Rendering;
using PSSlideShow.Rendering;

namespace PSSlideShow.Model {
    /// <summary>A QR matrix rendered as fixed-size terminal half-blocks, without resampling modules.</summary>
    public sealed class QrCodeWidget : SlideWidget {
        private static readonly Style ModuleStyle = Style.Parse("black on white");
        private readonly bool[,] _modules;

        internal QrCodeWidget(bool[,] modules) {
            if (modules == null) throw new ArgumentNullException(nameof(modules));
            if (modules.GetLength(0) == 0 || modules.GetLength(0) != modules.GetLength(1)) {
                throw new ArgumentException("The QR matrix must be nonempty and square.", nameof(modules));
            }

            _modules = (bool[,])modules.Clone();
            ModuleCount = _modules.GetLength(0);
            VerticalPadding = 0;
        }

        public int ModuleCount { get; }

        public override IRenderable Render(WidgetRenderContext context) => new QrCodeRenderable(this, context);

        private sealed class QrCodeRenderable : IRenderable {
            private readonly QrCodeWidget _widget;
            private readonly WidgetRenderContext _context;

            public QrCodeRenderable(QrCodeWidget widget, WidgetRenderContext context) {
                _widget = widget;
                _context = context;
            }

            public Measurement Measure(RenderOptions options, int maxWidth) => new Measurement(0, maxWidth);

            public IEnumerable<Segment> Render(RenderOptions options, int maxWidth) {
                var width = Math.Max(0, Math.Min(_context.Width, maxWidth));
                var height = options.Height.HasValue
                    ? Math.Min(_context.Height, options.Height.Value)
                    : _context.Height;
                var availableRows = Math.Max(0L, (long)height - 2L * Math.Max(0, _widget.VerticalPadding));
                var scale = (int)Math.Min(
                    width / _widget.ModuleCount,
                    Math.Min(int.MaxValue, availableRows * 2 / _widget.ModuleCount));

                if (scale < 1) {
                    var minimumRows = (_widget.ModuleCount + 1) / 2 + 2L * Math.Max(0, _widget.VerticalPadding);
                    return ((IRenderable)new Text(
                        $"QR code needs {_widget.ModuleCount} columns and {minimumRows} content rows; enlarge the terminal.",
                        _context.Theme.Error)).Render(options, Math.Max(1, width));
                }

                var size = _widget.ModuleCount * scale;
                var left = (width - size) / 2;
                var renderedRows = (size + 1) / 2;
                var topRows = (int)((availableRows - renderedRows) / 2);
                var lines = new List<SegmentLine>(topRows + renderedRows);
                for (var i = 0; i < topRows; i++) {
                    lines.Add(new SegmentLine { new Segment(new string(' ', width)) });
                }

                for (var y = 0; y < size; y += 2) {
                    var cells = new char[size];
                    for (var x = 0; x < size; x++) {
                        var top = _widget._modules[y / scale, x / scale];
                        var bottom = y + 1 < size && _widget._modules[(y + 1) / scale, x / scale];
                        cells[x] = top
                            ? (bottom ? '\u2588' : '\u2580')
                            : (bottom ? '\u2584' : ' ');
                    }

                    var line = new SegmentLine();
                    if (left > 0) {
                        line.Add(new Segment(new string(' ', left)));
                    }

                    line.Add(new Segment(new string(cells), ModuleStyle));
                    lines.Add(line);
                }

                return new RenderedLines(lines).Render(options, width);
            }
        }
    }
}
