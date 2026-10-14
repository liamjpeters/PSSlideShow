using System.Collections.Generic;
using System.Management.Automation;
using PSSlideShow.Authoring;
using PSSlideShow.Model;

namespace PSSlideShow.Cmdlets.Widgets {
    /// <summary>Shared collection logic for the layout container cmdlets.</summary>
    public abstract class LayoutContainerCommandBase : WidgetCommand {
        /// <summary>Script block whose <c>New-SlideColumn</c>/<c>New-SlideRow</c> calls build the regions.</summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ScriptBlock Body { get; set; } = null!;

        /// <summary>Use a fixed-size layout that clips instead of a content-sized flow layout.</summary>
        [Parameter]
        public SwitchParameter Fill { get; set; }

        protected abstract LayoutOrientation Orientation { get; }

        protected override void EndProcessing() {
            var regions = new List<LayoutRegion>();
            using (SlideBuildContext.Push(Orientation.ToString())) {
                foreach (var result in Body.Invoke()) {
                    switch (result?.BaseObject) {
                        case LayoutRegion region:
                            regions.Add(region);
                            break;
                        case ISlideWidget widget:
                            regions.Add(new LayoutRegion(new[] { widget }));
                            break;
                    }
                }
            }

            WriteWidget(new LayoutWidget(Orientation, regions, Fill.IsPresent));
        }
    }

    /// <summary>
    /// <para type="synopsis">Arranges regions side-by-side.</para>
    /// <para type="description">Body should contain <c>New-SlideColumn</c> calls.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideColumns")]
    [OutputType(typeof(LayoutWidget))]
    public sealed class NewSlideColumnsCommand : LayoutContainerCommandBase {
        protected override LayoutOrientation Orientation => LayoutOrientation.Columns;
    }

    /// <summary>
    /// <para type="synopsis">Arranges regions stacked vertically.</para>
    /// <para type="description">Body should contain <c>New-SlideRow</c> calls.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideRows")]
    [OutputType(typeof(LayoutWidget))]
    public sealed class NewSlideRowsCommand : LayoutContainerCommandBase {
        protected override LayoutOrientation Orientation => LayoutOrientation.Rows;
    }

    /// <summary>Shared collection logic for the layout region cmdlets.</summary>
    public abstract class LayoutRegionCommandBase : PSCmdlet {
        /// <summary>Script block whose widgets fill this region.</summary>
        [Parameter(Position = 0)]
        public ScriptBlock? Body { get; set; }

        /// <summary>Proportional share of the available space (Fill/columns only).</summary>
        [Parameter]
        public int? Ratio { get; set; }

        /// <summary>Fixed size in cells (Fill/columns only).</summary>
        [Parameter]
        public int? Size { get; set; }

        /// <summary>Minimum size in cells (Fill/columns only).</summary>
        [Parameter]
        public int? MinSize { get; set; }

        protected override void EndProcessing() {
            var widgets = new List<ISlideWidget>();
            if (Body != null) {
                using (SlideBuildContext.Push("Region")) {
                    foreach (var result in Body.Invoke()) {
                        if (result?.BaseObject is ISlideWidget widget) {
                            widgets.Add(widget);
                        }
                    }
                }
            }

            WriteObject(new LayoutRegion(widgets, Ratio, Size, MinSize));
        }
    }

    /// <summary>
    /// <para type="synopsis">Defines a column region inside <c>New-SlideColumns</c>.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideColumn")]
    [OutputType(typeof(LayoutRegion))]
    public sealed class NewSlideColumnCommand : LayoutRegionCommandBase {
    }

    /// <summary>
    /// <para type="synopsis">Defines a row region inside <c>New-SlideRows</c>.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideRow")]
    [OutputType(typeof(LayoutRegion))]
    public sealed class NewSlideRowCommand : LayoutRegionCommandBase {
    }
}
