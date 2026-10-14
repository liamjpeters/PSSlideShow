using System.Management.Automation;
using PSSlideShow.Model;

namespace PSSlideShow.Cmdlets.Widgets {
    /// <summary>
    /// <para type="synopsis">Adds a bulleted or numbered list to a slide.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideList")]
    [OutputType(typeof(ListWidget))]
    public sealed class NewSlideListCommand : WidgetCommand {
        /// <summary>List items (each may contain Spectre markup).</summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string[] Items { get; set; } = System.Array.Empty<string>();

        /// <summary>Render <c>1.</c>, <c>2.</c> … instead of bullets.</summary>
        [Parameter]
        public SwitchParameter Numbered { get; set; }

        /// <summary>Custom bullet glyph (default <c>•</c>). Ignored when <c>-Numbered</c>.</summary>
        [Parameter]
        public string Marker { get; set; } = "•";

        protected override void EndProcessing() =>
            WriteWidget(new ListWidget(Items, Numbered.IsPresent, Marker));
    }
}
