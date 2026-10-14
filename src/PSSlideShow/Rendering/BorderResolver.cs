using Spectre.Console;
using PSSlideShow.Model;

namespace PSSlideShow.Rendering {
    /// <summary>Maps the module's <see cref="SlideBorder"/> enum to a Spectre <see cref="TableBorder"/>.</summary>
    public static class BorderResolver {
        public static TableBorder Resolve(SlideBorder border) => border switch {
            SlideBorder.None => TableBorder.None,
            SlideBorder.Ascii => TableBorder.Ascii,
            SlideBorder.Square => TableBorder.Square,
            SlideBorder.Rounded => TableBorder.Rounded,
            SlideBorder.Heavy => TableBorder.Heavy,
            SlideBorder.Double => TableBorder.Double,
            SlideBorder.Minimal => TableBorder.Minimal,
            _ => TableBorder.Rounded
        };
    }
}
