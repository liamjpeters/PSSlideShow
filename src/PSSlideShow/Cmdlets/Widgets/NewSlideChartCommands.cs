using System.Collections.Generic;
using System.Management.Automation;
using Spectre.Console;
using PSSlideShow.Authoring;
using PSSlideShow.Model;

namespace PSSlideShow.Cmdlets.Widgets {
    /// <summary>
    /// <para type="synopsis">Adds a horizontal bar chart to a slide.</para>
    /// <para type="description">
    /// Items may be hashtables (<c>@{ Label = 'A'; Value = 10; Color = 'Green' }</c>) or objects with
    /// matching properties, supplied via <c>-Item</c> or the pipeline.
    /// </para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideBarChart")]
    [OutputType(typeof(BarChartWidget))]
    public sealed class NewSlideBarChartCommand : WidgetCommand {
        /// <summary>Chart items (hashtables or objects with Label/Value/Color).</summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public object[] Item { get; set; } = System.Array.Empty<object>();

        /// <summary>Optional chart heading.</summary>
        [Parameter]
        public string? Label { get; set; }

        /// <summary>Heading alignment. Default <c>Center</c>.</summary>
        [Parameter]
        public Justify LabelAlignment { get; set; } = Justify.Center;

        /// <summary>Fixes the value axis maximum (otherwise inferred from the data).</summary>
        [Parameter]
        public double? MaxValue { get; set; }

        /// <summary>Hides the numeric value next to each bar.</summary>
        [Parameter]
        public SwitchParameter HideValues { get; set; }

        /// <summary>Script block that formats each value (value is <c>$_</c>).</summary>
        [Parameter]
        public ScriptBlock? ValueFormatter { get; set; }

        /// <summary>Chart width in cells (defaults to the content width).</summary>
        [Parameter]
        public int? Width { get; set; }

        private readonly List<object> _items = new();

        protected override void ProcessRecord() {
            foreach (var item in Item) {
                if (item != null) {
                    _items.Add(item);
                }
            }
        }

        protected override void EndProcessing() {
            var items = ChartItem.ParseMany(_items);
            WriteWidget(new BarChartWidget(
                items,
                Label,
                LabelAlignment,
                MaxValue,
                HideValues.IsPresent,
                ScriptBlockFormatter.ToFunc(ValueFormatter),
                Width));
        }
    }

    /// <summary>
    /// <para type="synopsis">Adds a proportional breakdown chart to a slide.</para>
    /// <para type="description">
    /// Items may be hashtables or objects with Label/Value/Color, supplied via <c>-Item</c> or the pipeline.
    /// </para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideBreakdownChart")]
    [OutputType(typeof(BreakdownChartWidget))]
    public sealed class NewSlideBreakdownChartCommand : WidgetCommand {
        /// <summary>Chart items (hashtables or objects with Label/Value/Color).</summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public object[] Item { get; set; } = System.Array.Empty<object>();

        /// <summary>Shows each slice as a percentage.</summary>
        [Parameter]
        public SwitchParameter ShowPercentage { get; set; }

        /// <summary>Hides the tag (legend) row.</summary>
        [Parameter]
        public SwitchParameter HideTags { get; set; }

        /// <summary>Hides the value shown next to each tag.</summary>
        [Parameter]
        public SwitchParameter HideTagValues { get; set; }

        /// <summary>Renders the full-size (non-compact) layout.</summary>
        [Parameter]
        public SwitchParameter FullSize { get; set; }

        /// <summary>Script block that formats each value (value is <c>$_</c>).</summary>
        [Parameter]
        public ScriptBlock? ValueFormatter { get; set; }

        /// <summary>Chart width in cells (defaults to the content width).</summary>
        [Parameter]
        public int? Width { get; set; }

        private readonly List<object> _items = new();

        protected override void ProcessRecord() {
            foreach (var item in Item) {
                if (item != null) {
                    _items.Add(item);
                }
            }
        }

        protected override void EndProcessing() {
            var items = ChartItem.ParseMany(_items);
            WriteWidget(new BreakdownChartWidget(
                items,
                ShowPercentage.IsPresent,
                HideTags.IsPresent,
                HideTagValues.IsPresent,
                FullSize.IsPresent,
                ScriptBlockFormatter.ToFunc(ValueFormatter),
                Width));
        }
    }
}
