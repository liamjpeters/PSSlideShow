using System;
using System.Collections.Generic;
using System.Management.Automation;
using PSSlideShow.Authoring;
using PSSlideShow.Model;

namespace PSSlideShow.Cmdlets.Widgets {
    /// <summary>
    /// <para type="synopsis">Adds a hierarchical tree to a slide.</para>
    /// <para type="description">
    /// The <c>-Body</c> script block contributes child nodes via nested <c>New-SlideTreeNode</c> calls.
    /// </para>
    /// </summary>
    /// <example>
    ///   <code>
    ///   New-SlideTree -Label 'src' -Body {
    ///       New-SlideTreeNode -Label 'Cmdlets'
    ///       New-SlideTreeNode -Label 'Model' -Body {
    ///           New-SlideTreeNode -Label 'Widgets'
    ///       }
    ///   }
    ///   </code>
    /// </example>
    [Cmdlet(VerbsCommon.New, "SlideTree")]
    [OutputType(typeof(TreeWidget))]
    public sealed class NewSlideTreeCommand : WidgetCommand {
        /// <summary>The root label. Supports Spectre markup.</summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string Label { get; set; } = string.Empty;

        /// <summary>Script block whose <c>New-SlideTreeNode</c> calls build the tree.</summary>
        [Parameter(Mandatory = true, Position = 1)]
        public ScriptBlock Body { get; set; } = null!;

        /// <summary>Branch guide glyph style.</summary>
        [Parameter]
        public SlideTreeGuide Guide { get; set; } = SlideTreeGuide.Line;

        /// <summary>Guide line color name/hex. Defaults to the low-contrast chrome style.</summary>
        [Parameter]
        public string? GuideColor { get; set; }

        /// <summary>Render the top-level nodes collapsed.</summary>
        [Parameter]
        public SwitchParameter Collapsed { get; set; }

        protected override void EndProcessing() {
            var nodes = new List<TreeNodeWidget>();
            using (SlideBuildContext.Push("Tree")) {
                foreach (var result in Body.Invoke()) {
                    if (result?.BaseObject is TreeNodeWidget node) {
                        nodes.Add(node);
                    }
                }
            }

            WriteWidget(new TreeWidget(Label, nodes, Guide, GuideColor, Collapsed.IsPresent));
        }
    }

    /// <summary>
    /// <para type="synopsis">Adds a node to the enclosing tree.</para>
    /// <para type="description">
    /// Only valid inside a <c>New-SlideTree</c> (or another <c>New-SlideTreeNode</c>) body. Supply a
    /// <c>-Body</c> to nest further child nodes.
    /// </para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideTreeNode")]
    [OutputType(typeof(TreeNodeWidget))]
    public sealed class NewSlideTreeNodeCommand : PSCmdlet {
        /// <summary>The node label. Supports Spectre markup.</summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string Label { get; set; } = string.Empty;

        /// <summary>Optional script block of nested <c>New-SlideTreeNode</c> calls.</summary>
        [Parameter(Position = 1)]
        public ScriptBlock? Body { get; set; }

        /// <summary>Render this node collapsed (hide its children).</summary>
        [Parameter]
        public SwitchParameter Collapsed { get; set; }

        protected override void EndProcessing() {
            if (!SlideBuildContext.InScope("Tree")) {
                ThrowTerminatingError(new ErrorRecord(
                    new InvalidOperationException(
                        "New-SlideTreeNode can only be used inside a New-SlideTree (or New-SlideTreeNode) body."),
                    "TreeNodeOutsideTree",
                    ErrorCategory.InvalidOperation,
                    null));
            }

            var children = new List<TreeNodeWidget>();
            if (Body != null) {
                using (SlideBuildContext.Push("TreeNode")) {
                    foreach (var result in Body.Invoke()) {
                        if (result?.BaseObject is TreeNodeWidget node) {
                            children.Add(node);
                        }
                    }
                }
            }

            WriteObject(new TreeNodeWidget(Label, children, Collapsed.IsPresent));
        }
    }
}
