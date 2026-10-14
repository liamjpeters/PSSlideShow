using Spectre.Console;

namespace PSSlideShow.Rendering {
    /// <summary>A small rotating palette used when a chart item does not specify its own color.</summary>
    public static class Palette {
        private static readonly Color[] Colors =
        {
            Color.Blue, Color.Green, Color.Yellow, Color.Red,
            Color.Aqua, Color.Fuchsia, Color.Orange1, Color.Purple
        };

        public static Color At(int index) {
            var i = ((index % Colors.Length) + Colors.Length) % Colors.Length;
            return Colors[i];
        }
    }
}
