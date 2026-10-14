using System.Collections;
using PSSlideShow.Model;
using Xunit;

namespace PSSlideShow.Tests.Widgets {
    public class TableWidgetTests {
        [Fact]
        public void Infers_columns_from_hashtable_keys() {
            var rows = new object[]
            {
                new Hashtable { { "Name", "Alice" }, { "Age", 30 } }
            };
            var output = TestSupport.Render(new TableWidget(rows), width: 60);
            Assert.Contains("Name", output);
            Assert.Contains("Age", output);
            Assert.Contains("Alice", output);
            Assert.Contains("30", output);
        }

        [Fact]
        public void Explicit_columns_control_order_and_subset() {
            var rows = new object[]
            {
                new Hashtable { { "Name", "Alice" }, { "Age", 30 } }
            };
            var output = TestSupport.Render(new TableWidget(rows, new[] { "Age" }), width: 60);
            Assert.Contains("Age", output);
            Assert.Contains("30", output);
            Assert.DoesNotContain("Alice", output);
        }
    }
}
