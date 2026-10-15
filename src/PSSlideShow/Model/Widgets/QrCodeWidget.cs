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

        public override IRenderable Render(WidgetRenderContext context) {
            var availableRows = Math.Max(0L, (long)context.Height - 2L * Math.Max(0, VerticalPadding));
            var scale = (int)Math.Min(
                context.Width / ModuleCount,
                Math.Min(int.MaxValue, availableRows * 2 / ModuleCount));

            if (scale < 1) {
                var minimumRows = (ModuleCount + 1) / 2 + 2L * Math.Max(0, VerticalPadding);
                return new Text(
                    $"QR code needs {ModuleCount} columns and {minimumRows} content rows; enlarge the terminal.",
                    context.Theme.Error);
            }

            var size = ModuleCount * scale;
            var left = (context.Width - size) / 2;
            var lines = new List<SegmentLine>((size + 1) / 2);
            for (var y = 0; y < size; y += 2) {
                var cells = new char[size];
                for (var x = 0; x < size; x++) {
                    var top = _modules[y / scale, x / scale];
                    var bottom = y + 1 < size && _modules[(y + 1) / scale, x / scale];
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

            return new RenderedLines(lines);
        }
    }
}
