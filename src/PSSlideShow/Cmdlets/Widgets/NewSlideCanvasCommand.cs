using System.Collections.Generic;
using System.Management.Automation;
using Spectre.Console;
using PSSlideShow.Model;

namespace PSSlideShow.Cmdlets.Widgets {
    /// <summary>
    /// <para type="synopsis">Adds a pixel canvas to a slide.</para>
    /// <para type="description">
    /// The <c>-Paint</c> script block runs at authoring time with the canvas exposed as <c>$_</c>
    /// (also <c>$PSItem</c>) and the dimensions as <c>$Width</c>/<c>$Height</c>. Use
    /// <c>$_.SetPixel(x, y, color)</c> and <c>$_.Fill(color)</c> to draw.
    /// </para>
    /// </summary>
    /// <example>
    ///   <code>
    ///   New-SlideCanvas -Width 16 -Height 16 -Paint {
    ///       $_.Fill('grey15')
    ///       for ($i = 0; $i -lt $Width; $i++) { $_.SetPixel($i, $i, 'lime') }
    ///   }
    ///   </code>
    /// </example>
    [Cmdlet(VerbsCommon.New, "SlideCanvas")]
    [OutputType(typeof(CanvasWidget))]
    public sealed class NewSlideCanvasCommand : WidgetCommand {
        /// <summary>Canvas width in pixels.</summary>
        [Parameter(Mandatory = true, Position = 0)]
        public int Width { get; set; }

        /// <summary>Canvas height in pixels.</summary>
        [Parameter(Mandatory = true, Position = 1)]
        public int Height { get; set; }

        /// <summary>Script block that paints the canvas ($_ is the canvas; $Width/$Height are available).</summary>
        [Parameter(Mandatory = true, Position = 2)]
        public ScriptBlock Paint { get; set; } = null!;

        /// <summary>Maximum rendered width in cells (defaults to the content width).</summary>
        [Parameter]
        public int? MaxWidth { get; set; }

        /// <summary>Disables auto-scaling to fit the available width.</summary>
        [Parameter]
        public SwitchParameter NoScale { get; set; }

        protected override void EndProcessing() {
            var canvas = new Canvas(Width, Height);
            var proxy = new SlideCanvas(canvas);

            var variables = new List<PSVariable>
            {
                new PSVariable("_", proxy),
                new PSVariable("PSItem", proxy),
                new PSVariable("Width", Width),
                new PSVariable("Height", Height)
            };

            Paint.InvokeWithContext(null, variables, proxy);

            WriteWidget(new CanvasWidget(proxy, MaxWidth, NoScale.IsPresent));
        }
    }
}
