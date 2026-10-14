using System;
using System.Management.Automation;
using PSSlideShow.Model;

namespace PSSlideShow.Cmdlets.Widgets {
    /// <summary>
    /// <para type="synopsis">Adds a periodically updated view to a slide.</para>
    /// <para type="description">
    /// The <c>-Update</c> script block runs in a dedicated background runspace and produces a snapshot.
    /// The <c>-View</c> script block receives that snapshot on the presentation thread and emits the
    /// widgets to render. The first update runs before the slide is initially rendered.
    /// </para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideDynamic")]
    [OutputType(typeof(DynamicWidget))]
    public sealed class NewSlideDynamicCommand : WidgetCommand {
        /// <summary>Time between completed background updates. Must be positive.</summary>
        [Parameter(Mandatory = true, Position = 0)]
        public TimeSpan Interval { get; set; }

        /// <summary>Script block that produces the snapshot passed to <c>-View</c>.</summary>
        [Parameter(Mandatory = true, Position = 1)]
        public ScriptBlock Update { get; set; } = null!;

        /// <summary>Script block that receives the latest snapshot and emits slide widgets.</summary>
        [Parameter(Mandatory = true, Position = 2)]
        public ScriptBlock View { get; set; } = null!;

        protected override void EndProcessing() {
            if (Interval <= TimeSpan.Zero) {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentOutOfRangeException(nameof(Interval), "The update interval must be positive."),
                    "InvalidDynamicInterval",
                    ErrorCategory.InvalidArgument,
                    Interval));
                return;
            }

            WriteWidget(new DynamicWidget(Interval, Update, View));
        }
    }
}