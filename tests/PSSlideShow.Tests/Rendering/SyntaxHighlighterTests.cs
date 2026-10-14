using System;
using System.Linq;
using PSSlideShow.Rendering;
using Xunit;

namespace PSSlideShow.Tests.Rendering {
    public class SyntaxHighlighterTests {
        private const string Code = "Get-Process | Sort-Object CPU";

        [Fact]
        public void Highlights_powershell_with_color_markup() {
            SyntaxHighlighter.UseTheme(light: false);
            var lines = SyntaxHighlighter.HighlightLines(Code, "powershell");

            Assert.NotEmpty(lines);
            // Highlighted output carries Spectre color markup like [#rrggbb]...[/].
            Assert.Contains(lines, line => line.Contains("[#") && line.Contains("[/]"));
        }

        [Fact]
        public void Dark_and_light_palettes_produce_different_colors() {
            SyntaxHighlighter.UseTheme(light: false);
            var dark = string.Join("\n", SyntaxHighlighter.HighlightLines(Code, "powershell"));

            SyntaxHighlighter.UseTheme(light: true);
            var light = string.Join("\n", SyntaxHighlighter.HighlightLines(Code, "powershell"));

            // Both should still contain the source text, but the color markup must differ so the
            // highlighting adapts to the terminal background.
            Assert.Contains("Get-Process", dark);
            Assert.Contains("Get-Process", light);
            Assert.NotEqual(dark, light);

            // Reset to the default so other tests are unaffected.
            SyntaxHighlighter.UseTheme(light: false);
        }

        [Theory]
        [InlineData("15;0", false)] // background index 0 => dark
        [InlineData("0;15", true)]  // background index 15 => light
        [InlineData("1;7", true)]   // background index 7 => light
        [InlineData("15;8", false)] // background index 8 => dark
        public void ColorFgBg_environment_is_honored(string value, bool expectedLight) {
            var previous = Environment.GetEnvironmentVariable("COLORFGBG");
            try {
                Environment.SetEnvironmentVariable("COLORFGBG", value);
                Assert.Equal(expectedLight, TerminalBackground.IsLight(TimeSpan.Zero));
            }
            finally {
                Environment.SetEnvironmentVariable("COLORFGBG", previous);
            }
        }
    }
}
