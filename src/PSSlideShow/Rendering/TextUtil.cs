using System;
using System.Collections.Generic;
using System.Linq;

namespace PSSlideShow.Rendering {
    /// <summary>Text helpers shared by widgets (line splitting and dedenting).</summary>
    public static class TextUtil {
        /// <summary>Splits text into lines, normalizing CRLF/CR to LF first.</summary>
        public static string[] SplitLines(string? text) =>
            (text ?? string.Empty).Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

        /// <summary>
        /// Removes leading/trailing blank lines and the longest run of leading whitespace common to
        /// every non-blank line, preserving relative indentation.
        /// </summary>
        public static string Dedent(string? code) {
            var lines = SplitLines(code).ToList();

            while (lines.Count > 0 && lines[0].Trim().Length == 0) lines.RemoveAt(0);
            while (lines.Count > 0 && lines[lines.Count - 1].Trim().Length == 0) lines.RemoveAt(lines.Count - 1);
            if (lines.Count == 0) return string.Empty;

            var min = int.MaxValue;
            foreach (var line in lines) {
                if (line.Trim().Length == 0) continue;
                var i = 0;
                while (i < line.Length && (line[i] == ' ' || line[i] == '\t')) i++;
                min = Math.Min(min, i);
            }

            if (min == int.MaxValue || min == 0) return string.Join("\n", lines);

            for (var i = 0; i < lines.Count; i++) {
                var line = lines[i];
                lines[i] = line.Length >= min ? line.Substring(min) : line.TrimStart();
            }

            return string.Join("\n", lines);
        }
    }
}
