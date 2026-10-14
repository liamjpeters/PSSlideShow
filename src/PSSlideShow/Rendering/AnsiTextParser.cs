using System.Collections.Generic;
using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PSSlideShow.Rendering {
    /// <summary>
    /// Parses a string containing ANSI SGR escape sequences (as emitted by PowerShell 7 formatting via
    /// <c>$PSStyle</c>) into Spectre.Console <see cref="SegmentLine"/>s. Colour and decoration are carried
    /// on each <see cref="Segment"/>'s <see cref="Style"/> rather than left as raw escape bytes in the
    /// text, so Spectre measures width by the visible characters only — the coloured output renders
    /// correctly and its width accounting stays exact (no border drift or leftover fragments).
    /// </summary>
    internal static class AnsiTextParser {
        private const char Escape = '\u001b';

        public static List<SegmentLine> ToLines(string? text) {
            var lines = new List<SegmentLine>();
            var line = new SegmentLine();
            var run = new StringBuilder();

            Color? foreground = null;
            Color? background = null;
            var decoration = Decoration.None;

            void FlushRun() {
                if (run.Length == 0) {
                    return;
                }

                line.Add(new Segment(run.ToString(), new Style(foreground, background, decoration)));
                run.Clear();
            }

            if (string.IsNullOrEmpty(text)) {
                lines.Add(line);
                return lines;
            }

            for (var i = 0; i < text!.Length; i++) {
                var ch = text[i];

                if (ch == '\r') {
                    continue;
                }

                if (ch == '\n') {
                    FlushRun();
                    lines.Add(line);
                    line = new SegmentLine();
                    continue;
                }

                // A CSI sequence: ESC '[' parameters final-byte. Only SGR ('m') affects style; any other
                // CSI (cursor moves, erases, ...) is consumed and ignored so it never leaks into the text.
                if (ch == Escape && i + 1 < text.Length && text[i + 1] == '[') {
                    var j = i + 2;
                    var start = j;
                    while (j < text.Length && text[j] != 'm' && !IsCsiFinal(text[j])) {
                        j++;
                    }

                    if (j < text.Length && text[j] == 'm') {
                        FlushRun();
                        ApplySgr(text.Substring(start, j - start), ref foreground, ref background, ref decoration);
                    }

                    // Skip the whole sequence (including the final byte if present).
                    i = j < text.Length ? j : text.Length - 1;
                    continue;
                }

                run.Append(ch);
            }

            FlushRun();
            lines.Add(line);
            return lines;
        }

        private static bool IsCsiFinal(char c) => c >= '@' && c <= '~';

        private static void ApplySgr(string parameters, ref Color? foreground, ref Color? background, ref Decoration decoration) {
            // An empty parameter list (ESC[m) is equivalent to a reset.
            if (parameters.Length == 0) {
                foreground = null;
                background = null;
                decoration = Decoration.None;
                return;
            }

            var codes = parameters.Split(';');
            for (var k = 0; k < codes.Length; k++) {
                if (!int.TryParse(codes[k], out var code)) {
                    continue;
                }

                switch (code) {
                    case 0:
                        foreground = null;
                        background = null;
                        decoration = Decoration.None;
                        break;
                    case 1: decoration |= Decoration.Bold; break;
                    case 2: decoration |= Decoration.Dim; break;
                    case 3: decoration |= Decoration.Italic; break;
                    case 4: decoration |= Decoration.Underline; break;
                    case 5: decoration |= Decoration.SlowBlink; break;
                    case 7: decoration |= Decoration.Invert; break;
                    case 8: decoration |= Decoration.Conceal; break;
                    case 9: decoration |= Decoration.Strikethrough; break;
                    case 22: decoration &= ~(Decoration.Bold | Decoration.Dim); break;
                    case 23: decoration &= ~Decoration.Italic; break;
                    case 24: decoration &= ~Decoration.Underline; break;
                    case 25: decoration &= ~Decoration.SlowBlink; break;
                    case 27: decoration &= ~Decoration.Invert; break;
                    case 28: decoration &= ~Decoration.Conceal; break;
                    case 29: decoration &= ~Decoration.Strikethrough; break;
                    case 39: foreground = null; break;
                    case 49: background = null; break;
                    case 38:
                        foreground = ReadExtendedColor(codes, ref k) ?? foreground;
                        break;
                    case 48:
                        background = ReadExtendedColor(codes, ref k) ?? background;
                        break;
                    default:
                        if (code >= 30 && code <= 37) foreground = Color.FromInt32(code - 30);
                        else if (code >= 90 && code <= 97) foreground = Color.FromInt32(code - 90 + 8);
                        else if (code >= 40 && code <= 47) background = Color.FromInt32(code - 40);
                        else if (code >= 100 && code <= 107) background = Color.FromInt32(code - 100 + 8);
                        break;
                }
            }
        }

        /// <summary>Reads a <c>38</c>/<c>48</c> extended colour: <c>5;n</c> (256-palette) or <c>2;r;g;b</c> (truecolor).</summary>
        private static Color? ReadExtendedColor(string[] codes, ref int k) {
            if (k + 1 >= codes.Length || !int.TryParse(codes[k + 1], out var mode)) {
                return null;
            }

            if (mode == 5 && k + 2 < codes.Length && int.TryParse(codes[k + 2], out var index)) {
                k += 2;
                return Color.FromInt32(index);
            }

            if (mode == 2 && k + 4 < codes.Length
                && int.TryParse(codes[k + 2], out var r)
                && int.TryParse(codes[k + 3], out var g)
                && int.TryParse(codes[k + 4], out var b)) {
                k += 4;
                return new Color((byte)r, (byte)g, (byte)b);
            }

            return null;
        }
    }
}
