using System.Collections.Generic;
using PSSlideShow.Model;
using Xunit;

namespace PSSlideShow.Tests.Widgets {
    public class TreeWidgetTests {
        [Fact]
        public void Renders_root_and_nested_labels() {
            var nodes = new List<TreeNodeWidget>
            {
                new TreeNodeWidget("Cmdlets"),
                new TreeNodeWidget("Model", new[] { new TreeNodeWidget("Widgets") })
            };
            var output = TestSupport.Render(new TreeWidget("src", nodes));
            Assert.Contains("src", output);
            Assert.Contains("Cmdlets", output);
            Assert.Contains("Model", output);
            Assert.Contains("Widgets", output);
        }

        [Fact]
        public void Collapsed_node_hides_its_children() {
            var nodes = new List<TreeNodeWidget>
            {
                new TreeNodeWidget("Model", new[] { new TreeNodeWidget("Hidden") }, collapsed: true)
            };
            var output = TestSupport.Render(new TreeWidget("src", nodes));
            Assert.Contains("Model", output);
            Assert.DoesNotContain("Hidden", output);
        }
    }
}
