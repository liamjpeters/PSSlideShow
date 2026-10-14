using System.Management.Automation;
using Spectre.Console;
using PSSlideShow.Model;

namespace PSSlideShow.Cmdlets.Widgets {
    /// <summary>
    /// <para type="synopsis">Adds large FIGlet banner text to a slide.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideFiglet")]
    [OutputType(typeof(FigletWidget))]
    public sealed class NewSlideFigletCommand : WidgetCommand {
        /// <summary>Text to render as a banner.</summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string Text { get; set; } = string.Empty;

        /// <summary>Horizontal justification.</summary>
        [Parameter]
        public Justify Justify { get; set; } = Justify.Center;

        /// <summary>Optional color.</summary>
        [Parameter]
        public string? Foreground { get; set; }

        /// <summary>Optional path to a FIGlet font file; defaults to the built-in font.</summary>
        [Parameter]
        public string? FontPath { get; set; }

        protected override void EndProcessing() {
            var path = string.IsNullOrWhiteSpace(FontPath)
                ? null
                : GetUnresolvedProviderPathFromPSPath(FontPath);
            WriteWidget(new FigletWidget(Text, Justify, Foreground, path));
        }
    }
}
