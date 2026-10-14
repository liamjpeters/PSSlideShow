using Spectre.Console;
using Spectre.Console.Rendering;

namespace PSSlideShow.Rendering {
    /// <summary>Small helpers shared by widget renderers.</summary>
    internal static class RenderUtil {
        /// <summary>Maps a Spectre <see cref="Justify"/> to a <see cref="HorizontalAlignment"/>.</summary>
        public static HorizontalAlignment ToHorizontal(Justify justify) => justify switch {
            Justify.Center => HorizontalAlignment.Center,
            Justify.Right => HorizontalAlignment.Right,
            _ => HorizontalAlignment.Left
        };

        /// <summary>Wraps a renderable so it is aligned within the full available width.</summary>
        public static IRenderable Aligned(IRenderable renderable, Justify justify) =>
            justify == Justify.Left ? renderable : new Align(renderable, ToHorizontal(justify));
    }
}
