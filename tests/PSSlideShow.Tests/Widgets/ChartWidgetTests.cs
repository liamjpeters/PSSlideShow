using System.Collections;
using System.Collections.Generic;
using PSSlideShow.Model;
using Xunit;

namespace PSSlideShow.Tests.Widgets {
    public class ChartWidgetTests {
        private static IReadOnlyList<ChartItem> Items() => new List<ChartItem>
        {
            new ChartItem("Alpha", 40, "green"),
            new ChartItem("Beta", 60, "blue")
        };

        [Fact]
        public void BarChart_renders_labels_and_values() {
            var output = TestSupport.Render(new BarChartWidget(Items(), label: "Scores"));
            Assert.Contains("Scores", output);
            Assert.Contains("Alpha", output);
            Assert.Contains("Beta", output);
            Assert.Contains("40", output);
        }

        [Fact]
        public void BreakdownChart_renders_tags() {
            var output = TestSupport.Render(new BreakdownChartWidget(Items()));
            Assert.Contains("Alpha", output);
            Assert.Contains("Beta", output);
        }

        [Fact]
        public void ChartItem_parses_hashtable() {
            var parsed = ChartItem.ParseMany(new object[]
            {
                new Hashtable { { "Label", "One" }, { "Value", 12 }, { "Color", "red" } }
            });
            Assert.Single(parsed);
            Assert.Equal("One", parsed[0].Label);
            Assert.Equal(12d, parsed[0].Value);
            Assert.Equal("red", parsed[0].Color);
        }
    }
}
