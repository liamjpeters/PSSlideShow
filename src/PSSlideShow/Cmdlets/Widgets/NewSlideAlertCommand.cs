using System.Management.Automation;
using PSSlideShow.Model;

namespace PSSlideShow.Cmdlets.Widgets {
    /// <summary>
    /// <para type="synopsis">Adds a GitHub-style alert callout to a slide.</para>
    /// <para type="description">Renders a coloured left bar, a coloured title (with a severity symbol),
    /// and body text below it. The colour and symbol come from the <c>-Severity</c>: Note (blue),
    /// Tip (green), Important (purple), Warning (amber) and Caution (red). The body text supports
    /// Spectre console markup (e.g. <c>[bold]</c>, <c>[green]</c>).</para>
    /// <example>
    ///   <code>New-SlideAlert -Severity Warning -Text 'This action cannot be undone.'</code>
    ///   <para>An amber warning callout titled "Warning".</para>
    /// </example>
    /// <example>
    ///   <code>New-SlideAlert -Severity Tip -Title 'Pro tip' -Text 'Press [green]Enter[/] to run the code.'</code>
    ///   <para>A green tip with a custom title and markup in the body.</para>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideAlert")]
    [OutputType(typeof(AlertWidget))]
    public sealed class NewSlideAlertCommand : WidgetCommand {
        /// <summary>The alert severity, which selects the colour and symbol.</summary>
        [Parameter(Mandatory = true, Position = 0)]
        public AlertSeverity Severity { get; set; } = AlertSeverity.Note;

        /// <summary>The body text (may contain Spectre markup).</summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string Text { get; set; } = string.Empty;

        /// <summary>Optional heading. Defaults to the severity name (e.g. "Warning").</summary>
        [Parameter(Position = 2)]
        public string? Title { get; set; }

        /// <summary>Hides the severity symbol before the title.</summary>
        [Parameter]
        public SwitchParameter NoSymbol { get; set; }

        protected override void EndProcessing() =>
            WriteWidget(new AlertWidget(Severity, Text, Title, !NoSymbol.IsPresent));
    }
}
