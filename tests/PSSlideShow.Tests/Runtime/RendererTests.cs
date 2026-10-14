using System.Collections.Generic;
using System.Linq;
using PSSlideShow.Model;
using PSSlideShow.Rendering;
using Xunit;

namespace PSSlideShow.Tests.Runtime {
    public class RendererTests {
        private static SlideShow Show(params Slide[] slides) => new SlideShow(slides);

        private static Slide TextSlide(string title, string text) =>
            new Slide(title, new ISlideWidget[] { new TextWidget(text) });

        [Fact]
        public void Renders_title_content_and_progress() {
            var console = TestSupport.Console(80, 20);
            var show = Show(
                TextSlide("First Slide", "Hello content"),
                TextSlide("Second Slide", "More"));

            new Renderer(console).Render(show);
            var output = console.Output;

            Assert.Contains("First Slide", output);
            Assert.Contains("Hello content", output);
            Assert.Contains("[01/02]", output);
        }

        [Fact]
        public void Shows_scroll_hint_when_content_overflows() {
            var console = TestSupport.Console(80, 10);
            var items = Enumerable.Range(1, 40).Select(i => $"Item {i}").ToArray();
            var slide = new Slide("Long", new ISlideWidget[] { new ListWidget(items) });

            new Renderer(console).Render(new SlideShow(new[] { slide }));

            Assert.Contains("Scroll", console.Output);
        }

        [Fact]
        public void Fill_slide_does_not_show_scroll_hint() {
            var console = TestSupport.Console(80, 10);
            var region = new LayoutRegion(new ISlideWidget[] { new TextWidget("panel") });
            var layout = new LayoutWidget(LayoutOrientation.Columns, new[] { region }, fill: true);
            var slide = new Slide("Dash", new ISlideWidget[] { layout });

            new Renderer(console).Render(new SlideShow(new[] { slide }));

            Assert.DoesNotContain("Scroll", console.Output);
        }

        [Fact]
        public void Horizontal_padding_indents_widget_content() {
            var console = TestSupport.Console(80, 20);
            var widget = new TextWidget("MARKER") { HorizontalPadding = 6, VerticalPadding = 0 };
            var slide = new Slide("T", new ISlideWidget[] { widget });

            new Renderer(console).Render(new SlideShow(new[] { slide }));

            var line = console.Lines.First(l => l.Contains("MARKER"));
            var indent = line.Length - line.TrimStart().Length;
            Assert.True(indent >= 6, $"expected at least 6 leading spaces, got {indent}: '{line}'");
        }

        [Fact]
        public void Vertical_padding_separates_stacked_widgets() {
            var console = TestSupport.Console(80, 20);
            var slide = new Slide("T", new ISlideWidget[]
            {
                new TextWidget("ALPHA"),
                new TextWidget("BETA"),
            });

            new Renderer(console).Render(new SlideShow(new[] { slide }));

            var lines = console.Lines.ToList();
            var a = lines.FindIndex(l => l.Contains("ALPHA"));
            var b = lines.FindIndex(l => l.Contains("BETA"));
            Assert.True(a >= 0 && b > a, $"markers not found in order: alpha={a} beta={b}");
            Assert.Contains(lines.Skip(a + 1).Take(b - a - 1), string.IsNullOrWhiteSpace);
        }

        [Fact]
        public void No_padding_leaves_content_flush_left() {
            var console = TestSupport.Console(80, 20);
            var widget = new TextWidget("FLUSH") { HorizontalPadding = 0, VerticalPadding = 0 };
            var slide = new Slide("T", new ISlideWidget[] { widget });

            new Renderer(console, contentPadding: 0).Render(new SlideShow(new[] { slide }));

            var line = console.Lines.First(l => l.Contains("FLUSH"));
            Assert.StartsWith("FLUSH", line.TrimEnd());
        }

        [Fact]
        public void Content_padding_insets_the_viewport_but_not_the_chrome() {
            var console = TestSupport.Console(80, 20);
            var widget = new TextWidget("BODY") { HorizontalPadding = 0, VerticalPadding = 0 };
            var slide = new Slide("Heading", new ISlideWidget[] { widget });

            new Renderer(console, contentPadding: 3).Render(new SlideShow(new[] { slide }));

            var lines = console.Lines.ToList();
            var body = lines.First(l => l.Contains("BODY"));
            var indent = body.Length - body.TrimStart().Length;
            Assert.True(indent >= 3, $"content should be inset by >= 3, got {indent}: '{body}'");

            var title = lines.First(l => l.Contains("Heading"));
            Assert.StartsWith("Heading", title.TrimEnd());
        }
    }
}
