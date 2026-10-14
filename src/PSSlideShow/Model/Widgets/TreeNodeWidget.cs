using System.Collections.Generic;

namespace PSSlideShow.Model {
    /// <summary>Selects the glyphs used to draw a tree's branch guide lines.</summary>
    public enum SlideTreeGuide {
        Line,
        Ascii,
        DoubleLine,
        BoldLine
    }

    /// <summary>
    /// A node in a <see cref="TreeWidget"/>. Emitted by <c>New-SlideTreeNode</c> and collected by
    /// its parent tree/node. Not a slide widget on its own &#8212; it is only meaningful inside a tree.
    /// </summary>
    public sealed class TreeNodeWidget {
        public TreeNodeWidget(string label, IReadOnlyList<TreeNodeWidget>? children = null, bool collapsed = false) {
            Label = label ?? string.Empty;
            Children = children ?? new List<TreeNodeWidget>();
            Collapsed = collapsed;
        }

        /// <summary>The node label. Supports Spectre console markup.</summary>
        public string Label { get; }

        /// <summary>Child nodes, in authoring order.</summary>
        public IReadOnlyList<TreeNodeWidget> Children { get; }

        /// <summary>When true, this node renders collapsed (its children are hidden).</summary>
        public bool Collapsed { get; }
    }
}
