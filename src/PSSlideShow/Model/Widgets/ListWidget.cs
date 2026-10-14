using System.Collections.Generic;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PSSlideShow.Model {
    /// <summary>A bulleted or numbered list. Each item may contain Spectre markup.</summary>
    public sealed class ListWidget : SlideWidget {
        public ListWidget(IReadOnlyList<string> items, bool numbered = false, string marker = "•") {
            Items = items ?? new List<string>();
            Numbered = numbered;
            Marker = string.IsNullOrEmpty(marker) ? "•" : marker;
        }

        public IReadOnlyList<string> Items { get; }
        public bool Numbered { get; }
        public string Marker { get; }
        public override bool IsRunnable => false;

        public override IRenderable Render(WidgetRenderContext context) {
            if (Items.Count == 0) {
                return new Text(string.Empty);
            }

            // Two columns — a narrow marker column and a text column that wraps within its own width —
            // so that wrapped continuation lines hang under the item text rather than under the marker.
            // Numbered markers share the widest prefix width, keeping every item's text left-aligned.
            var grid = new Grid();
            grid.AddColumn(new GridColumn().NoWrap().Padding(0, 0, 1, 0));
            grid.AddColumn(new GridColumn().Padding(0, 0, 0, 0));

            for (var i = 0; i < Items.Count; i++) {
                var prefix = Numbered ? $"{i + 1}." : Marker;
                grid.AddRow(new Markup(prefix), new Markup(Items[i]));
            }

            return grid;
        }
    }
}
