using PSSlideShow.Model;
using PSSlideShow.Rendering;
using Spectre.Console.Testing;

namespace PSSlideShow.Tests {
    /// <summary>Shared helpers for rendering widgets against a deterministic <see cref="TestConsole"/>.</summary>
    internal static class TestSupport {
        /// <summary>Creates a <see cref="TestConsole"/> with a fixed width for reproducible output.</summary>
        public static TestConsole Console(int width = 80, int height = 30) {
            var console = new TestConsole();
            console.Profile.Width = width;
            console.Profile.Height = height;
            return console;
        }

        /// <summary>Renders a widget to a fixed-width console and returns the emitted text.</summary>
        public static string Render(ISlideWidget widget, int width = 80, int height = 30) {
            var console = Console(width, height);
            var context = new WidgetRenderContext(console, width, height, SlideTheme.Default);
            console.Write(widget.Render(context));
            return console.Output;
        }

        /// <summary>Renders an arbitrary Spectre renderable to a fixed-width console and returns the text.</summary>
        public static string RenderRaw(Spectre.Console.Rendering.IRenderable renderable, int width = 80, int height = 30) {
            var console = Console(width, height);
            console.Write(renderable);
            return console.Output;
        }
    }
}
