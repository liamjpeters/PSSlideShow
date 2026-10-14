using System.Collections.Generic;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PSSlideShow.Rendering {
    /// <summary>
    /// Renders a slide's content once to <see cref="SegmentLine"/>s so the frame renderer can measure
    /// its height, slice a scroll window, and re-emit only the visible lines (preserving color).
    /// </summary>
    internal static class ContentCapture {
        public static List<SegmentLine> Capture(IAnsiConsole console, IRenderable renderable, int width, int? height) {
            var size = new Size(width, height ?? 1);
            var options = new RenderOptions(console.Profile.Capabilities, size) {
                Height = height
            };

            var segments = renderable.Render(options, width);
            return Segment.SplitLines(segments);
        }
    }

    /// <summary>An <see cref="IRenderable"/> that re-emits a pre-captured, pre-sliced set of segment lines.</summary>
    internal sealed class RenderedLines : IRenderable {
        private readonly IReadOnlyList<SegmentLine> _lines;

        public RenderedLines(IReadOnlyList<SegmentLine> lines) {
            _lines = lines;
        }

        public Measurement Measure(RenderOptions options, int maxWidth) => new Measurement(0, maxWidth);

        public IEnumerable<Segment> Render(RenderOptions options, int maxWidth) {
            foreach (var line in _lines) {
                foreach (var segment in line) {
                    yield return segment;
                }

                yield return Segment.LineBreak;
            }
        }
    }

    /// <summary>
    /// Emits a fixed-size grid of segment lines, clipping every line to exactly <c>width</c> cells and
    /// filling up to <c>height</c> rows. Every physical row is written as precisely <c>width</c> cells —
    /// overflow is truncated (so a too-wide line can never wrap onto the next row) and any shortfall is
    /// padded with spaces. Because every cell in the region is repainted with no wrapping, the frame can
    /// be drawn over the previous one by simply homing the cursor: there is never a leftover character to
    /// erase, and nothing bleeds onto an adjacent row.
    /// </summary>
    internal sealed class PaddedFrame : IRenderable {
        private readonly IReadOnlyList<SegmentLine> _lines;
        private readonly int _width;
        private readonly int _height;

        public PaddedFrame(IReadOnlyList<SegmentLine> lines, int width, int height) {
            _lines = lines;
            _width = width;
            _height = height;
        }

        public Measurement Measure(RenderOptions options, int maxWidth) => new Measurement(_width, _width);

        public IEnumerable<Segment> Render(RenderOptions options, int maxWidth) {
            for (var row = 0; row < _height; row++) {
                var remaining = _width;
                if (row < _lines.Count) {
                    foreach (var segment in _lines[row]) {
                        if (segment.IsLineBreak) {
                            continue;
                        }

                        if (remaining <= 0) {
                            break;
                        }

                        var length = segment.CellCount();
                        if (length <= remaining) {
                            yield return segment;
                            remaining -= length;
                        }
                        else {
                            // The segment would overflow the viewport: clip it so it can never wrap onto
                            // (and corrupt) the next physical row.
                            var text = segment.Text;
                            if (text.Length > remaining) {
                                text = text.Substring(0, remaining);
                            }

                            yield return new Segment(text, segment.Style);
                            remaining = 0;
                        }
                    }
                }

                if (remaining > 0) {
                    yield return new Segment(new string(' ', remaining));
                }

                yield return Segment.LineBreak;
            }
        }
    }
}
