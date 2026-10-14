using System;
using Spectre.Console;

namespace PSSlideShow.Rendering {
    /// <summary>
    /// Parses friendly color inputs (palette names, hex, <see cref="ConsoleColor"/>, or RGB) into a
    /// Spectre <see cref="Color"/>. Named colors resolve through Spectre's palette, so they stay
    /// theme-aware; hex/RGB request 24-bit truecolor and degrade gracefully on limited terminals.
    /// </summary>
    public static class ColorResolver {
        /// <summary>Parses a name (<c>cyan1</c>), hex (<c>#00d7ff</c>) or <c>rgb(r,g,b)</c> string.</summary>
        public static Color Parse(string value) {
            if (string.IsNullOrWhiteSpace(value)) return Color.Default;
            // Style.Parse understands names, #rrggbb, and rgb(...) forms.
            return Style.Parse(value.Trim()).Foreground;
        }

        /// <summary>Resolves an arbitrary object (string, ConsoleColor, or the RGB overloads' bytes).</summary>
        public static Color Resolve(object? value) {
            switch (value) {
                case null:
                    return Color.Default;
                case Color color:
                    return color;
                case ConsoleColor consoleColor:
                    return FromConsoleColor(consoleColor);
                case string s:
                    return Parse(s);
                default:
                    return Parse(value.ToString() ?? string.Empty);
            }
        }

        public static Color FromRgb(int r, int g, int b) =>
            new Color((byte)Clamp(r), (byte)Clamp(g), (byte)Clamp(b));

        public static Color FromConsoleColor(ConsoleColor color) => color switch {
            ConsoleColor.Black => Color.Black,
            ConsoleColor.DarkBlue => Color.Navy,
            ConsoleColor.DarkGreen => Color.Green,
            ConsoleColor.DarkCyan => Color.Teal,
            ConsoleColor.DarkRed => Color.Maroon,
            ConsoleColor.DarkMagenta => Color.Purple,
            ConsoleColor.DarkYellow => Color.Olive,
            ConsoleColor.Gray => Color.Silver,
            ConsoleColor.DarkGray => Color.Grey,
            ConsoleColor.Blue => Color.Blue,
            ConsoleColor.Green => Color.Lime,
            ConsoleColor.Cyan => Color.Aqua,
            ConsoleColor.Red => Color.Red,
            ConsoleColor.Magenta => Color.Fuchsia,
            ConsoleColor.Yellow => Color.Yellow,
            ConsoleColor.White => Color.White,
            _ => Color.Default
        };

        private static int Clamp(int v) => v < 0 ? 0 : (v > 255 ? 255 : v);
    }
}
