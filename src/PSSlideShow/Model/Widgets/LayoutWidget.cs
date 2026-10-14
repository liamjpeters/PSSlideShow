using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PSSlideShow.Model {
    /// <summary>Whether a layout arranges its regions side-by-side or stacked.</summary>
    public enum LayoutOrientation {
        Columns,
        Rows
    }

    /// <summary>
    /// One region of a <see cref="LayoutWidget"/>. Emitted by <c>New-SlideColumn</c>/<c>New-SlideRow</c>
    /// and holds the widgets contributed by its body. Sizing hints (<c>-Size</c>/<c>-Ratio</c>/<c>-MinSize</c>)
    /// apply to flow columns and in <c>-Fill</c> mode; flow rows always stack and size to content.
    /// </summary>
    public sealed class LayoutRegion {
        public LayoutRegion(
            IReadOnlyList<ISlideWidget> widgets,
            int? ratio = null,
            int? size = null,
            int? minSize = null) {
            Widgets = widgets ?? new List<ISlideWidget>();
            Ratio = ratio;
            Size = size;
            MinSize = minSize;
        }

        public IReadOnlyList<ISlideWidget> Widgets { get; }
        public int? Ratio { get; }
        public int? Size { get; }
        public int? MinSize { get; }

        internal IRenderable Render(WidgetRenderContext context) {
            if (Widgets.Count == 0) {
                return new Text(string.Empty);
            }

            if (Widgets.Count == 1) {
                return Widgets[0].Render(context);
            }

            return new Rows(Widgets.Select(w => w.Render(context)).ToArray());
        }
    }

    /// <summary>
    /// A container that arranges regions. In flow mode (default) columns take an equal share of the width
    /// (or a per-region <c>-Size</c>/<c>-Ratio</c>) via a Spectre <c>Grid</c> and rows stack with <c>Rows</c>;
    /// in <c>-Fill</c> mode it uses a fixed-size <see cref="Layout"/> that clips.
    /// </summary>
    public sealed class LayoutWidget : SlideWidget {
        public LayoutWidget(LayoutOrientation orientation, IReadOnlyList<LayoutRegion> regions, bool fill = false) {
            Orientation = orientation;
            Regions = regions ?? new List<LayoutRegion>();
            Fill = fill;
        }

        public LayoutOrientation Orientation { get; }
        public IReadOnlyList<LayoutRegion> Regions { get; }
        public bool Fill { get; }
        public override bool IsRunnable => false;

        public override IRenderable Render(WidgetRenderContext context) {
            if (Regions.Count == 0) {
                return new Text(string.Empty);
            }

            return Fill ? BuildFill(this, context) : BuildFlow(context);
        }

        private IRenderable BuildFlow(WidgetRenderContext context) {
            if (Orientation == LayoutOrientation.Rows) {
                return new Rows(Regions.Select(r => r.Render(context)).ToArray());
            }

            // Columns get a fixed share of the width (equal by default, or per -Size/-Ratio) and each
            // region is rendered constrained to that share. A Spectre Grid enforces the split so wide
            // widgets wrap inside their column instead of pushing the next column onto a new row.
            var widths = ComputeColumnWidths(context.Width);
            var grid = new Grid { Expand = true };
            for (var i = 0; i < Regions.Count; i++) {
                var column = new GridColumn();
                if (widths[i] > 0) {
                    column.Width = widths[i];
                }

                grid.AddColumn(column);
            }

            grid.AddRow(Regions
                .Select((r, i) => r.Render(context.WithWidth(widths[i] > 0 ? widths[i] : context.Width)))
                .ToArray());
            return grid;
        }

        private int[] ComputeColumnWidths(int total) {
            var widths = new int[Regions.Count];
            var remaining = total;
            var ratioIndices = new List<int>();
            var ratioTotal = 0;

            for (var i = 0; i < Regions.Count; i++) {
                var region = Regions[i];
                if (region.Size.HasValue) {
                    widths[i] = region.Size.Value;
                    remaining -= region.Size.Value;
                }
                else {
                    var ratio = region.Ratio ?? 1;
                    ratioTotal += ratio;
                    ratioIndices.Add(i);
                }
            }

            if (remaining < 0) {
                remaining = 0;
            }

            foreach (var i in ratioIndices) {
                var ratio = Regions[i].Ratio ?? 1;
                var width = ratioTotal > 0 ? remaining * ratio / ratioTotal : 0;
                var min = Regions[i].MinSize ?? 0;
                widths[i] = width < min ? min : width;
            }

            return widths;
        }

        private static Layout BuildFill(LayoutWidget widget, WidgetRenderContext context) {
            var children = widget.Regions
                .Select((region, index) => BuildFillRegion(region, index, context))
                .ToArray();

            var root = new Layout();
            if (widget.Orientation == LayoutOrientation.Columns) {
                root.SplitColumns(children);
            }
            else {
                root.SplitRows(children);
            }

            return root;
        }

        private static Layout BuildFillRegion(LayoutRegion region, int index, WidgetRenderContext context) {
            var layout = new Layout($"region-{index}");
            if (region.Ratio.HasValue) {
                layout.Ratio = region.Ratio.Value;
            }

            if (region.Size.HasValue) {
                layout.Size = region.Size.Value;
            }

            if (region.MinSize.HasValue) {
                layout.MinimumSize = region.MinSize.Value;
            }

            if (region.Widgets.Count == 1 && region.Widgets[0] is LayoutWidget inner && inner.Fill) {
                layout.Update(BuildFill(inner, context));
            }
            else {
                layout.Update(region.Render(context));
            }

            return layout;
        }
    }
}
