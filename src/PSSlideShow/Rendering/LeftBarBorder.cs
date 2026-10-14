using Spectre.Console;
using Spectre.Console.Rendering;

namespace PSSlideShow.Rendering {
    /// <summary>
    /// A <see cref="BoxBorder"/> that draws only a solid vertical bar down the left edge (like a
    /// GitHub-style alert accent). Every other border part renders as a space, so a <see cref="Panel"/>
    /// using this border shows just a full-height coloured bar to the left of its content.
    /// </summary>
    public sealed class LeftBarBorder : BoxBorder {
        /// <summary>A thick solid left half-block used as the bar glyph.</summary>
        private const string Bar = "\u258c"; // ▌

        public override string GetPart(BoxBorderPart part) => part switch {
            BoxBorderPart.TopLeft => Bar,
            BoxBorderPart.Left => Bar,
            BoxBorderPart.BottomLeft => Bar,
            _ => " ",
        };
    }
}
