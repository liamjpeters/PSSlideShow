using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace PSSlideShow.Rendering {
    /// <summary>
    /// How the syntax-highlighting palette should be chosen for code blocks.
    /// </summary>
    public enum SyntaxThemePreference {
        /// <summary>Detect the terminal background and pick a light or dark palette automatically.</summary>
        Auto,

        /// <summary>Always use the dark-background palette.</summary>
        Dark,

        /// <summary>Always use the light-background palette.</summary>
        Light
    }

    /// <summary>
    /// Best-effort detection of whether the terminal has a light or dark background, so the syntax
    /// highlighter (which uses absolute RGB colors) can pick a legible palette. Detection is
    /// deliberately conservative: any uncertainty returns <see langword="null"/> and callers fall back
    /// to a dark palette.
    /// </summary>
    public static class TerminalBackground {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMilliseconds(150);

        // OSC 11 responses look like: ESC ] 11 ; rgb:RRRR/GGGG/BBBB  (terminated by BEL or ESC \).
        private static readonly Regex Osc11Rgb = new Regex(
            @"rgb:([0-9a-fA-F]+)/([0-9a-fA-F]+)/([0-9a-fA-F]+)",
            RegexOptions.CultureInvariant);

        /// <summary>
        /// Returns <see langword="true"/> if the terminal background is light, <see langword="false"/>
        /// if dark, or <see langword="null"/> if it could not be determined.
        /// </summary>
        public static bool? IsLight() => IsLight(DefaultTimeout);

        /// <summary>
        /// Returns <see langword="true"/> if the terminal background is light, <see langword="false"/>
        /// if dark, or <see langword="null"/> if it could not be determined within
        /// <paramref name="osc11Timeout"/>.
        /// </summary>
        public static bool? IsLight(TimeSpan osc11Timeout) {
            // COLORFGBG (set by some terminals/tmux) is cheap and needs no round-trip.
            var fromEnv = FromColorFgBg();
            if (fromEnv.HasValue) return fromEnv;

            // Otherwise ask the terminal directly with an OSC 11 query.
            return FromOsc11(osc11Timeout);
        }

        private static bool? FromColorFgBg() {
            var value = Environment.GetEnvironmentVariable("COLORFGBG");
            if (string.IsNullOrWhiteSpace(value)) return null;

            // Format is "foreground;background" or "foreground;default;background"; the background is
            // the last field, as an ANSI palette index.
            var parts = value.Split(';');
            var last = parts[parts.Length - 1].Trim();
            if (!int.TryParse(last, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index)) {
                return null;
            }

            // 0-6 and 8 are the dark palette entries; 7 and 9-15 are the light ones.
            return index == 7 || index >= 9;
        }

        private static bool? FromOsc11(TimeSpan timeout) {
            if (Console.IsInputRedirected || Console.IsOutputRedirected) {
                return null;
            }

            try {
                // Query the background color; the terminal echoes it back on stdin.
                Console.Out.Write("\x1b]11;?\x07");
                Console.Out.Flush();

                var deadline = DateTime.UtcNow + timeout;
                var response = new StringBuilder();
                var started = false;

                while (DateTime.UtcNow < deadline) {
                    if (Console.KeyAvailable) {
                        var ch = Console.ReadKey(intercept: true).KeyChar;
                        response.Append(ch);
                        started = true;

                        // Response terminates with BEL (0x07) or ST (ESC \).
                        if (ch == '\x07') break;
                        if (ch == '\\' && response.Length >= 2 && response[response.Length - 2] == '\x1b') break;
                    }
                    else {
                        // Poll a little faster once bytes have started arriving.
                        Thread.Sleep(started ? 1 : 5);
                    }
                }

                return ParseOsc11(response.ToString());
            }
            catch {
                // Any host that doesn't support the query (or throws reading input) is treated as unknown.
                return null;
            }
        }

        private static bool? ParseOsc11(string response) {
            var match = Osc11Rgb.Match(response);
            if (!match.Success) return null;

            var r = ScaleHexChannel(match.Groups[1].Value);
            var g = ScaleHexChannel(match.Groups[2].Value);
            var b = ScaleHexChannel(match.Groups[3].Value);

            // Relative luminance (sRGB coefficients). A bright background reads as "light".
            var luminance = (0.2126 * r) + (0.7152 * g) + (0.0722 * b);
            return luminance >= 0.5;
        }

        private static double ScaleHexChannel(string hex) {
            // Channels may be 1-4 hex digits (e.g. rgb:f/f/f or rgb:ffff/ffff/ffff); normalize to 0..1.
            var max = Math.Pow(16, hex.Length) - 1;
            if (max <= 0) return 0;
            var value = (double)Convert.ToInt64(hex, 16);
            return value / max;
        }
    }
}
