using System.Collections.Generic;

namespace PSSlideShow.Model {
    /// <summary>
    /// Collects the runnable <see cref="CodeWidget"/>s of a slide in top-to-bottom order, descending
    /// into layout containers so nested code blocks join the sequential-run flow.
    /// </summary>
    internal static class RunnableCollector {
        public static List<CodeWidget> Collect(Slide slide) {
            var result = new List<CodeWidget>();
            if (slide != null) {
                Walk(slide.Widgets, result);
            }

            return result;
        }

        private static void Walk(IEnumerable<ISlideWidget> widgets, List<CodeWidget> accumulator) {
            foreach (var widget in widgets) {
                switch (widget) {
                    case CodeWidget code when code.IsRunnable:
                        accumulator.Add(code);
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
