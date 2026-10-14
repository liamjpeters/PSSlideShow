using System.Linq;
using PSSlideShow.Model;
using PSSlideShow.Rendering;
using Xunit;

namespace PSSlideShow.Tests.Widgets {
    public class ListWidgetTests {
        [Fact]
        public void Bulleted_list_uses_marker() {
            var output = TestSupport.Render(new ListWidget(new[] { "one", "two" }));
            Assert.Contains("• one", output);
            Assert.Contains("• two", output);
        }

        [Fact]
        public void Numbered_list_uses_indices() {
            var output = TestSupport.Render(new ListWidget(new[] { "one", "two" }, numbered: true));
            Assert.Contains("1. one", output);
            Assert.Contains("2. two", output);
        }

        [Fact]
        public void Custom_marker_is_used() {
            var output = TestSupport.Render(new ListWidget(new[] { "x" }, marker: "->"));
            Assert.Contains("-> x", output);
        }

        [Fact]
        public void Wrapped_item_text_hangs_under_the_first_line() {
            var console = TestSupport.Console(20, 10);
            var context = new WidgetRenderContext(console, 20, 10, SlideTheme.Default);
            var widget = new ListWidget(new[] { "alpha beta gamma delta epsilon zeta" });

            console.Write(widget.Render(context));

            var lines = console.Lines.Where(l => l.TrimEnd().Length > 0).ToList();
            Assert.True(lines.Count >= 2, "long item should wrap to a second line");
            Assert.StartsWith("• ", lines[0]);
            // Continuation lines are indented under the text (marker width + gap = 2), not under the bullet.
            Assert.DoesNotContain('•', lines[1]);
            Assert.Equal(2, lines[1].Length - lines[1].TrimStart().Length);
        }
    }
}
