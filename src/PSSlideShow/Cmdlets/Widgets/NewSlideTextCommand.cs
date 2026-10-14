using System.Management.Automation;
using Spectre.Console;
using PSSlideShow.Model;

namespace PSSlideShow.Cmdlets.Widgets {
    /// <summary>
    /// <para type="synopsis">Adds a paragraph of text to a slide.</para>
    /// <para type="description">Supports Spectre console markup (e.g. <c>[bold]</c>, <c>[green]</c>).</para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideText")]
    [OutputType(typeof(TextWidget))]
    public sealed class NewSlideTextCommand : WidgetCommand {
        /// <summary>The text (may contain Spectre markup).</summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string Text { get; set; } = string.Empty;

        /// <summary>Horizontal justification.</summary>
        [Parameter]
        public Justify Justify { get; set; } = Justify.Left;

        /// <summary>Optional foreground color name/hex.</summary>
        [Parameter]
        public string? Foreground { get; set; }

        protected override void EndProcessing() =>
            WriteWidget(new TextWidget(Text, Justify, Foreground));
    }
}
