using System;
using System.Collections.Generic;
using Spectre.Console;
using Spectre.Console.Rendering;
using PSSlideShow.Rendering;

namespace PSSlideShow.Model {
    /// <summary>A horizontal bar chart backed by Spectre's <see cref="BarChart"/>.</summary>
    public sealed class BarChartWidget : SlideWidget {
        public BarChartWidget(
            IReadOnlyList<ChartItem> items,
            string? label = null,
            Justify labelAlignment = Justify.Center,
            double? maxValue = null,
            bool hideValues = false,
            Func<double, string>? valueFormatter = null,
            int? width = null) {
            Items = items ?? new List<ChartItem>();
            Label = label;
            LabelAlignment = labelAlignment;
            MaxValue = maxValue;
            HideValues = hideValues;
            ValueFormatter = valueFormatter;
            Width = width;
        }

        public IReadOnlyList<ChartItem> Items { get; }
        public string? Label { get; }
        public Justify LabelAlignment { get; }
        public double? MaxValue { get; }
        public bool HideValues { get; }
        public Func<double, string>? ValueFormatter { get; }
        public int? Width { get; }
        public override bool IsRunnable => false;

        public override IRenderable Render(WidgetRenderContext context) {
            var chart = new BarChart {
                LabelAlignment = LabelAlignment,
                ShowValues = !HideValues
            };

            if (!string.IsNullOrEmpty(Label)) {
                chart.Label = Label;
            }

            if (MaxValue.HasValue) {
                chart.MaxValue = MaxValue.Value;
            }

            chart.Width = Width ?? context.Width;

            if (ValueFormatter != null) {
                chart.UseValueFormatter(ValueFormatter);
            }

            for (var i = 0; i < Items.Count; i++) {
                var item = Items[i];
                var color = item.Color != null ? ColorResolver.Parse(item.Color) : Palette.At(i);
                chart.AddItem(item.Label, item.Value, color);
            }

            return chart;
        }
    }
}
