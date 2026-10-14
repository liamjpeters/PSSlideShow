using System.Collections.Generic;

namespace PSSlideShow.Model {
    /// <summary>Collects dynamic widgets from a slide, descending through layout regions.</summary>
    internal static class DynamicCollector {
        public static List<DynamicWidget> Collect(Slide slide) {
            var result = new List<DynamicWidget>();
            if (slide != null) {
                Walk(slide.Widgets, result);
            }

            return result;
        }

        private static void Walk(IEnumerable<ISlideWidget> widgets, List<DynamicWidget> accumulator) {
            foreach (var widget in widgets) {
                switch (widget) {
                    case DynamicWidget dynamic:
                        accumulator.Add(dynamic);
                        break;
                    case LayoutWidget layout:
                        foreach (var region in layout.Regions) {
                            Walk(region.Widgets, accumulator);
                        }

                        break;
                }
            }
        }
    }
}