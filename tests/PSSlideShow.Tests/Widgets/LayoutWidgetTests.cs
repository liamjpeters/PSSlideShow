using System.Collections.Generic;
using PSSlideShow.Model;
using PSSlideShow.Rendering;
using Xunit;

namespace PSSlideShow.Tests.Widgets {
    public class LayoutWidgetTests {
        private static LayoutRegion Region(string text, int? ratio = null, int? size = null)
            => new LayoutRegion(new ISlideWidget[] { new TextWidget(text) }, ratio, size);

        [Fact]
        public void Flow_columns_render_all_regions() {
            var widget = new LayoutWidget(
                LayoutOrientation.Columns,
                new[] { Region("LEFT"), Region("RIGHT") });
            var output = TestSupport.Render(widget, width: 60);
            Assert.Contains("LEFT", output);
            Assert.Contains("RIGHT", output);
        }

        [Fact]
        public void Flow_columns_place_regions_side_by_side() {
            var console = TestSupport.Console(60, 10);
            var context = new WidgetRenderContext(console, 60, 10, SlideTheme.Default);
            var widget = new LayoutWidget(
                LayoutOrientation.Columns,
                new[] { Region("LEFT"), Region("RIGHT") });

            console.Write(widget.Render(context));

            Assert.Contains(console.Lines, l => l.Contains("LEFT") && l.Contains("RIGHT"));
        }

        [Fact]
        public void Flow_columns_constrain_wide_regions_instead_of_stacking() {
            var console = TestSupport.Console(60, 12);
            var context = new WidgetRenderContext(console, 60, 12, SlideTheme.Default);
            var wide = new LayoutRegion(new ISlideWidget[]
            {
                new TextWidget("this is a fairly long paragraph that keeps going and going")
            });
            var widget = new LayoutWidget(
                LayoutOrientation.Columns,
                new[] { Region("MARK"), wide });

            console.Write(widget.Render(context));

            // The left marker shares a row with the start of the wide right column (side by side),
            // rather than the wide region wrapping onto its own rows below.
            Assert.Contains(console.Lines, l => l.Contains("MARK") && l.Contains("this"));
        }

        [Fact]
        public void Flow_rows_render_all_regions() {
            var widget = new LayoutWidget(
                LayoutOrientation.Rows,
                new[] { Region("TOP"), Region("BOTTOM") });
            var output = TestSupport.Render(widget, width: 60);
            Assert.Contains("TOP", output);
            Assert.Contains("BOTTOM", output);
        }

        [Fact]
        public void Sized_columns_use_a_grid_and_render_regions() {
            var widget = new LayoutWidget(
                LayoutOrientation.Columns,
                new[] { Region("A", size: 20), Region("B", ratio: 1) });
            var output = TestSupport.Render(widget, width: 60);
            Assert.Contains("A", output);
            Assert.Contains("B", output);
        }

        [Fact]
        public void Fill_layout_renders_all_regions() {
            var widget = new LayoutWidget(
                LayoutOrientation.Columns,
                new[] { Region("ONE", ratio: 1), Region("TWO", ratio: 2) },
                fill: true);
            var output = TestSupport.Render(widget, width: 60, height: 10);
            Assert.Contains("ONE", output);
            Assert.Contains("TWO", output);
        }
    }
}
