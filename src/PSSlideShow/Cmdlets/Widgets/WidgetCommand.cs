using System.Management.Automation;
using PSSlideShow.Model;

namespace PSSlideShow.Cmdlets.Widgets {
    /// <summary>
    /// Base class for the top-level widget cmdlets. Adds the shared <c>-HPadding</c> / <c>-VPadding</c>
    /// parameters and applies them to the produced widget, so every widget gets consistent, tunable
    /// spacing. Horizontal padding defaults to 0; vertical padding defaults to 1 for out-of-the-box
    /// separation between widgets.
    /// </summary>
    public abstract class WidgetCommand : PSCmdlet {
        /// <summary>Left/right padding in cells around the widget. Defaults to 0.</summary>
        [Parameter]
        public int HPadding { get; set; } = 0;

        /// <summary>Top/bottom padding in rows around the widget. Defaults to 1.</summary>
        [Parameter]
        public int VPadding { get; set; } = 1;

        /// <summary>Applies the padding parameters to <paramref name="widget"/> and emits it.</summary>
        protected void WriteWidget(SlideWidget widget) {
            widget.HorizontalPadding = HPadding < 0 ? 0 : HPadding;
            widget.VerticalPadding = VPadding < 0 ? 0 : VPadding;
            WriteObject(widget);
        }
    }
}
