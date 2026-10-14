using PSSlideShow.Model;
using Spectre.Console;
using Xunit;

namespace PSSlideShow.Tests.Widgets {
    public class TextWidgetTests {
        [Fact]
        public void Renders_plain_text() {
            var output = TestSupport.Render(new TextWidget("Hello world"));
            Assert.Contains("Hello world", output);
        }

        [Fact]
        public void Strips_markup_to_plain_text() {
            var output = TestSupport.Render(new TextWidget("[bold]Bold[/] text"));
            Assert.Contains("Bold text", output);
            Assert.DoesNotContain("[bold]", output);
        }

        [Fact]
        public void Center_justify_pads_left() {
            var output = TestSupport.Render(new TextWidget("hi", Justify.Center), width: 20);
            // Centered text is preceded by leading whitespace.
            Assert.Matches(@"\s{3,}hi", output);
        }

        [Fact]
        public void Is_not_runnable() {
            Assert.False(new TextWidget("x").IsRunnable);
        }
    }
}
