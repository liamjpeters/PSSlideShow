using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using Spectre.Console.Rendering;
using PSSlideShow.Input;
using PSSlideShow.Model;

namespace PSSlideShow.Rendering {
    /// <summary>
    /// Composes and writes a single frame for the current slide: a pinned title, the scrolled content
    /// viewport, and the footer. Everything is written through the injected <see cref="IAnsiConsole"/>
    /// so the renderer is fully testable against a <c>TestConsole</c>.
    /// </summary>
    public sealed class Renderer {
        private const int TitleRows = 2;
        private const int FooterRows = 2;
        private const int DefaultContentPadding = 2;

        private readonly IAnsiConsole _console;
        private readonly SlideTheme _theme;
        private readonly FooterBuilder _footer;
        private readonly int _contentPadding;

        public Renderer(IAnsiConsole console, SlideTheme? theme = null, int contentPadding = DefaultContentPadding) {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _theme = theme ?? SlideTheme.Default;
            _footer = new FooterBuilder(_theme);
            _contentPadding = Math.Max(0, contentPadding);
        }

        /// <summary>Renders the show's current slide.</summary>
        public void Render(SlideShow show) {
            if (show?.Current == null) {
                return;
            }

            var slide = show.Current;
            var width = Math.Max(1, _console.Profile.Width);
            var totalHeight = Math.Max(TitleRows + FooterRows + 1, _console.Profile.Height);
            // Reserve the final terminal row: a frame that fills every row ends with a newline on the
            // last line, which scrolls the (alternate) buffer every render and makes some terminals
            // ring the bell. Leaving one row free keeps the cursor on-screen after each paint.
            var usableHeight = totalHeight - 1;
            var contentHeight = Math.Max(1, usableHeight - TitleRows - FooterRows);
            // Inset the content viewport from the terminal edges so widgets don't hug the leading column.
            // The title, rules and footer stay full-width; only the content block is guttered.
            var gutter = ContentGutter(width);
            var contentWidth = Math.Max(1, width - (2 * gutter));
            var context = new WidgetRenderContext(_console, contentWidth, contentHeight, _theme);

            var fill = IsFill(slide);
            var content = BuildContent(slide, context);
            var lines = ContentCapture.Capture(_console, content, contentWidth, fill ? contentHeight : (int?)null);

            var overflow = !fill && lines.Count > contentHeight;
            var maxOffset = Math.Max(0, lines.Count - contentHeight);
            var offset = fill ? 0 : Math.Min(Math.Max(0, show.ScrollOffset), maxOffset);
            if (!fill) {
                show.ScrollOffset = offset;
            }

            var visible = new List<SegmentLine>(contentHeight);
            for (var i = 0; i < contentHeight; i++) {
                var index = offset + i;
                var line = index < lines.Count ? lines[index] : new SegmentLine();
                visible.Add(Indent(line, gutter));
            }

            var hasPending = RunnableCollector.Collect(slide).Any(w => w.State.Status != CodeRunStatus.Ran);
            var state = new SlideState(show.IsFirst, show.IsLast, slide.HasRunnable || hasPending, hasPending, overflow);

            var frame = new Rows(
                new Text(slide.Title ?? string.Empty, _theme.Chrome),
                new Rule { Style = _theme.Chrome },
                new RenderedLines(visible),
                new Rule { Style = _theme.Chrome },
                _footer.Build(state, show.CurrentIndex + 1, show.Count, width));

            // Capture the composed frame and pad every cell to the full viewport. The host repaints by
            // homing the cursor (never clearing the whole screen), so full-coverage padding is what keeps
            // the redraw flicker-free: each frame overwrites the previous one in place.
            var frameLines = ContentCapture.Capture(_console, frame, width, usableHeight);
            _console.Write(new PaddedFrame(frameLines, width, usableHeight));
        }

        private IRenderable BuildContent(Slide slide, WidgetRenderContext context) {
            if (slide.Widgets.Count == 0) {
                return new Text(string.Empty);
            }

            if (slide.Widgets.Count == 1) {
                return RenderPadded(slide.Widgets[0], context);
            }

            return new Rows(slide.Widgets.Select(w => RenderPadded(w, context)).ToArray());
        }

        /// <summary>
        /// Renders a top-level widget and wraps it in its optional outer padding. Horizontal padding
        /// shrinks the width handed to the widget so the padded result still fits the viewport; vertical
        /// padding adds blank rows above/below for visual separation between widgets.
        /// </summary>
        private static IRenderable RenderPadded(ISlideWidget widget, WidgetRenderContext context) {
            var h = 0;
            var v = 0;
            if (widget is SlideWidget padded) {
                h = Math.Max(0, padded.HorizontalPadding);
                v = Math.Max(0, padded.VerticalPadding);
            }

            var inner = h > 0 ? context.WithWidth(Math.Max(1, context.Width - (2 * h))) : context;
            var rendered = widget.Render(inner);

            if (h == 0 && v == 0) {
                return rendered;
            }

            return new Padder(rendered, new Padding(h, v, h, v)) { Expand = true };
        }

        /// <summary>
        /// The effective left/right content gutter for a given viewport width, clamped so it never
        /// consumes the whole line on a very narrow terminal.
        /// </summary>
        private int ContentGutter(int width) => Math.Max(0, Math.Min(_contentPadding, (width - 1) / 2));

        /// <summary>Prepends <paramref name="spaces"/> blank cells to a captured content line.</summary>
        private static SegmentLine Indent(SegmentLine line, int spaces) {
            if (spaces <= 0) {
                return line;
            }

            var indented = new SegmentLine { new Segment(new string(' ', spaces)) };
            indented.AddRange(line);
            return indented;
        }

        private static bool IsFill(Slide slide) =>
            slide.Widgets.Count == 1 && slide.Widgets[0] is LayoutWidget { Fill: true };
    }
}
