using System.Collections.Generic;
using Spectre.Console;
using Spectre.Console.Rendering;
using PSSlideShow.Rendering;

namespace PSSlideShow.Model {
    /// <summary>
    /// A hierarchical tree. Authored with <c>New-SlideTree</c> whose body contributes
    /// <see cref="TreeNodeWidget"/> children via nested <c>New-SlideTreeNode</c> calls.
    /// </summary>
    public sealed class TreeWidget : SlideWidget {
        public TreeWidget(
            string label,
            IReadOnlyList<TreeNodeWidget> nodes,
            SlideTreeGuide guide = SlideTreeGuide.Line,
            string? guideColor = null,
            bool collapsed = false) {
            Label = label ?? string.Empty;
            Nodes = nodes ?? new List<TreeNodeWidget>();
            Guide = guide;
            GuideColor = guideColor;
            Collapsed = collapsed;
        }

        public string Label { get; }
        public IReadOnlyList<TreeNodeWidget> Nodes { get; }
        public SlideTreeGuide Guide { get; }
        public string? GuideColor { get; }
        public bool Collapsed { get; }
        public override bool IsRunnable => false;

        public override IRenderable Render(WidgetRenderContext context) {
            var tree = new Tree(new Markup(Label)) {
                Guide = ResolveGuide(Guide)
            };

            var guideStyle = GuideColor != null
                ? new Style(ColorResolver.Parse(GuideColor))
                : context.Theme.Chrome;
            tree.Style = guideStyle;

            foreach (var node in Nodes) {
                AddNode(tree, node, Collapsed);
            }

            return tree;
        }

        private static void AddNode(IHasTreeNodes parent, TreeNodeWidget node, bool forceCollapsed) {
            var treeNode = parent.AddNode(new Markup(node.Label));
            foreach (var child in node.Children) {
                AddNode(treeNode, child, false);
            }

            if (node.Collapsed || forceCollapsed) {
                treeNode.Expanded = false;
            }
        }

        private static TreeGuide ResolveGuide(SlideTreeGuide guide) => guide switch {
            SlideTreeGuide.Ascii => TreeGuide.Ascii,
            SlideTreeGuide.DoubleLine => TreeGuide.DoubleLine,
            SlideTreeGuide.BoldLine => TreeGuide.BoldLine,
            _ => TreeGuide.Line
        };
    }
}
