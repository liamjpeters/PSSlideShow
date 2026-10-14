using System;
using Spectre.Console;
using PSSlideShow.Rendering;

namespace PSSlideShow.Model {
    /// <summary>
    /// Everything a widget needs to render deterministically: the (injected) console, the available
    /// width/height, and the active theme. Passing these in — rather than reading global state —
    /// keeps widgets testable against a <c>TestConsole</c>.
    /// </summary>
    public sealed class WidgetRenderContext {
        public WidgetRenderContext(IAnsiConsole console, int width, int height, SlideTheme theme) {
            Console = console ?? throw new ArgumentNullException(nameof(console));
            Width = width;
            Height = height;
            Theme = theme ?? throw new ArgumentNullException(nameof(theme));
        }

        /// <summary>The console to render against (production console or a test double).</summary>
        public IAnsiConsole Console { get; }

        /// <summary>Available width in cells.</summary>
        public int Width { get; }

        /// <summary>Available height in rows (may be unbounded for flow content; see the renderer).</summary>
        public int Height { get; }

        /// <summary>The active color/theme roles.</summary>
        public SlideTheme Theme { get; }

        /// <summary>Creates a copy of this context with a different width (used by layout regions).</summary>
        public WidgetRenderContext WithWidth(int width) => new WidgetRenderContext(Console, width, Height, Theme);
    }
}
