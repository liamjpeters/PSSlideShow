using PSSlideShow.Model;
using Spectre.Console;
using Xunit;

namespace PSSlideShow.Tests.Widgets {
    public class CanvasWidgetTests {
        [Fact]
        public void Renders_painted_canvas() {
            var canvas = new Canvas(4, 4);
            var proxy = new SlideCanvas(canvas);
            proxy.Fill("blue");
            proxy.SetPixel(0, 0, "red");

            var output = TestSupport.Render(new CanvasWidget(proxy), width: 40, height: 20);
            Assert.True(output.Length > 0);
        }

        [Fact]
        public void SetPixel_ignores_out_of_bounds() {
            var canvas = new Canvas(2, 2);
            var proxy = new SlideCanvas(canvas);
            proxy.SetPixel(99, 99, "red");
            proxy.SetPixel(-1, 0, 10, 20, 30);

            var output = TestSupport.Render(new CanvasWidget(proxy), width: 40, height: 20);
            Assert.True(output.Length > 0);
        }
    }
}
