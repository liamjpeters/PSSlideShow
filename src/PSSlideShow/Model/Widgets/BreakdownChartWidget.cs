using System;
using System.Collections.Generic;
using Spectre.Console;
using Spectre.Console.Rendering;
using PSSlideShow.Rendering;

namespace PSSlideShow.Model {
    /// <summary>A proportional breakdown chart backed by Spectre's <see cref="BreakdownChart"/>.</summary>
    public sealed class BreakdownChartWidget : SlideWidget {
        public BreakdownChartWidget(
            IReadOnlyList<ChartItem> items,
            bool showPercentage = false,
            bool hideTags = false,
            bool hideTagValues = false,
            bool fullSize = false,
            Func<double, string>? valueFormatter = null,
            int? width = null) {
            Items = items ?? new List<ChartItem>();
            ShowPercentage = showPercentage;
            HideTags = hideTags;
            HideTagValues = hideTagValues;
            FullSize = fullSize;
            ValueFormatter = valueFormatter;
            Width = width;
        }

        public IReadOnlyList<ChartItem> Items { get; }
        public bool ShowPercentage { get; }
        public bool HideTags { get; }
        public bool HideTagValues { get; }
        public bool FullSize { get; }
        public Func<double, string>? ValueFormatter { get; }
        public int? Width { get; }
        public override bool IsRunnable => false;

        public override IRenderable Render(WidgetRenderContext context) {
            var chart = new BreakdownChart {
                ShowTags = !HideTags,
                ShowTagValues = !HideTagValues,
                Compact = !FullSize
            };

            chart.Width = Width ?? context.Width;

            if (ShowPercentage) {
                chart.ShowPercentage();
            }

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
