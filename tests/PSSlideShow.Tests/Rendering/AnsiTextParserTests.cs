using System.Linq;
using PSSlideShow.Rendering;
using Spectre.Console;
using Xunit;

namespace PSSlideShow.Tests.Rendering {
    public class AnsiTextParserTests {
        [Fact]
        public void Plain_text_becomes_a_single_unstyled_segment() {
            var lines = AnsiTextParser.ToLines("Hello World");

            var line = Assert.Single(lines);
            var segment = Assert.Single(line);
            Assert.Equal("Hello World", segment.Text);
            Assert.Equal(Color.Default, segment.Style.Foreground);
        }

        [Fact]
        public void Sgr_colour_is_applied_and_escapes_are_removed() {
            // ESC[32m green ESC[0m
            var lines = AnsiTextParser.ToLines("\u001b[32mHello\u001b[0m World");

            var line = Assert.Single(lines);
            var visible = string.Concat(line.Select(s => s.Text));
            Assert.Equal("Hello World", visible);
            Assert.DoesNotContain('\u001b', visible);

            var coloured = line.First(s => s.Text == "Hello");
            Assert.Equal(Color.Green, coloured.Style.Foreground);
        }

        [Fact]
        public void Bold_decoration_is_parsed() {
            var lines = AnsiTextParser.ToLines("\u001b[1mBold\u001b[0m");

            var segment = lines.Single().First(s => s.Text == "Bold");
            Assert.True(segment.Style.Decoration.HasFlag(Decoration.Bold));
        }

        [Fact]
        public void Truecolor_and_256_palette_are_parsed() {
            var rgb = AnsiTextParser.ToLines("\u001b[38;2;10;20;30mX\u001b[0m").Single().First(s => s.Text == "X");
            Assert.Equal(new Color(10, 20, 30), rgb.Style.Foreground);

            var indexed = AnsiTextParser.ToLines("\u001b[38;5;208mY\u001b[0m").Single().First(s => s.Text == "Y");
            Assert.Equal(Color.FromInt32(208), indexed.Style.Foreground);
        }

        [Fact]
        public void Newlines_split_into_separate_lines() {
            var lines = AnsiTextParser.ToLines("one\ntwo");

            Assert.Equal(2, lines.Count);
            Assert.Equal("one", string.Concat(lines[0].Select(s => s.Text)));
            Assert.Equal("two", string.Concat(lines[1].Select(s => s.Text)));
        }

        [Fact]
        public void Non_sgr_csi_sequences_are_consumed_not_rendered() {
            // ESC[2K (erase line) should vanish, leaving only the visible text.
            var lines = AnsiTextParser.ToLines("a\u001b[2Kb");

            Assert.Equal("ab", string.Concat(lines.Single().Select(s => s.Text)));
        }
    }
}
