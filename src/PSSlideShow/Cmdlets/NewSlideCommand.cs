using System.Collections.Generic;
using System.Management.Automation;
using PSSlideShow.Authoring;
using PSSlideShow.Model;

namespace PSSlideShow.Cmdlets {
    /// <summary>
    /// <para type="synopsis">Creates a single slide from a content script block.</para>
    /// <para type="description">
    /// Invokes the <c>-Content</c> script block and collects every <c>ISlideWidget</c> it emits (in
    /// order) as the slide's content stack. Widget cmdlets (<c>New-Slide*</c>) are only meaningful
    /// inside this block.
    /// </para>
    /// </summary>
    /// <example>
    ///   <code>
    ///   New-Slide -Title 'Welcome' -Content {
    ///       New-SlideFiglet -Text 'SlideShow'
    ///       New-SlideText   -Text 'A terminal slide show.' -Justify Center
    ///   }
    ///   </code>
    /// </example>
    [Cmdlet(VerbsCommon.New, "Slide")]
    [OutputType(typeof(Slide))]
    public sealed class NewSlideCommand : PSCmdlet {
        /// <summary>Optional title text. Empty/absent reserves a blank title row.</summary>
        [Parameter(Position = 0)]
        public string? Title { get; set; }

        /// <summary>The script block that emits the slide's widgets. Required.</summary>
        [Parameter(Mandatory = true, Position = 1)]
        public ScriptBlock Content { get; set; } = null!;

        /// <summary>Runspace-sharing policy for runnable code on this slide.</summary>
        [Parameter]
        public CodeScope CodeScope { get; set; } = CodeScope.Slide;

        protected override void EndProcessing() {
            var widgets = new List<ISlideWidget>();
            using (SlideBuildContext.Push("Slide")) {
                foreach (var result in Content.Invoke()) {
                    if (result?.BaseObject is ISlideWidget widget) {
                        widgets.Add(widget);
                    }
                }
            }

            WriteObject(new Slide(Title, widgets, CodeScope));
        }
    }
}
