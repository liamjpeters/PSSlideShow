using System.Globalization;

namespace PSSlideShow.Model {
    /// <summary>Derives a code block's title from its language id (and optional file name).</summary>
    public static class CodeTitle {
        public static string Derive(string? language, string? fileName = null) {
            var lang = (language ?? string.Empty).Trim().ToLowerInvariant();
            var title = lang switch {
                "powershell" => "PowerShell",
                "ps1" => "PowerShell",
                "csharp" => "C#",
                "cs" => "C#",
                "json" => "JSON",
                "" => "Text",
                _ => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lang)
            };

            if (!string.IsNullOrEmpty(fileName)) {
                title += " - " + fileName;
            }

            return title;
        }
    }
}
