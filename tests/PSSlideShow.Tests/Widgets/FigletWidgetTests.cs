using PSSlideShow.Model;
using Xunit;

namespace PSSlideShow.Tests.Widgets {
    public class FigletWidgetTests {
        [Fact]
        public void Renders_banner_larger_than_input() {
            var output = TestSupport.Render(new FigletWidget("Hi"), width: 80);
            // A FIGlet banner spans several rows.
            var lines = output.TrimEnd().Split('\n');
            Assert.True(lines.Length >= 3, "Expected a multi-row FIGlet banner.");
        }

        [Fact]
        public void Is_not_runnable() {
            Assert.False(new FigletWidget("x").IsRunnable);
        }
    }
}
