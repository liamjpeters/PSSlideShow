using System;
using System.Collections.Generic;
using System.Linq;
using PSSlideShow.Model;
using PSSlideShow.Rendering;
using QRCoder;
using Spectre.Console;
using Spectre.Console.Rendering;
using Xunit;

namespace PSSlideShow.Tests.Widgets {
    public class QrCodeWidgetTests {
        [Fact]
        public void Encoder_includes_a_four_module_white_quiet_zone() {
            using var qr = QRCodeGenerator.GenerateQrCode("HELLO", QRCodeGenerator.ECCLevel.M);
            var matrix = qr.ModuleMatrix;

            Assert.Equal(29, matrix.Count);
            foreach (var index in Enumerable.Range(0, matrix.Count)) {
                foreach (var border in Enumerable.Range(0, 4)) {
                    Assert.False(matrix[border][index]);
                    Assert.False(matrix[matrix.Count - 1 - border][index]);
                    Assert.False(matrix[index][border]);
                    Assert.False(matrix[index][matrix.Count - 1 - border]);
                }
            }
        }

        [Fact]
        public void Packs_two_modules_into_each_styled_terminal_cell() {
            var modules = new bool[29, 29];
            modules[4, 4] = true;
            modules[5, 5] = true;
            modules[4, 6] = true;
            modules[5, 6] = true;
            var widget = new QrCodeWidget(modules);
            var console = TestSupport.Console(29, 15);
            var context = new WidgetRenderContext(console, 29, 15, SlideTheme.Default);

            var captured = ContentCapture.Capture(console, widget.Render(context), 29, null);
            var lines = Lines(captured);

            Assert.Equal(15, lines.Count);
            Assert.All(lines, line => Assert.Equal(29, line.Length));
            Assert.Equal('\u2580', lines[2][4]);
            Assert.Equal('\u2584', lines[2][5]);
            Assert.Equal('\u2588', lines[2][6]);
            Assert.Equal(' ', lines[2][7]);
            Assert.Equal(Color.White, captured[0].Single().Style.Background);
            Assert.Equal(Color.Black, captured[0].Single().Style.Foreground);
        }

        [Fact]
        public void Resizing_changes_only_the_integer_module_scale() {
            var modules = new bool[29, 29];
            modules[4, 4] = true;
            var widget = new QrCodeWidget(modules);

            var small = RenderLines(widget, 29, 15);
            var almostWide = RenderLines(widget, 57, 29);
            var wide = RenderLines(widget, 58, 29);

            Assert.Equal(15, small.Count);
            Assert.Equal(15, almostWide.Count);
            Assert.Equal(29, wide.Count);
            Assert.Equal("\u2588\u2588", wide[4].Substring(8, 2));
            Assert.Equal(small, RenderLines(widget, 29, 15));
        }

        [Fact]
        public void Too_small_viewport_shows_a_message_instead_of_clipping_the_code() {
            var widget = new QrCodeWidget(new bool[29, 29]);

            var output = TestSupport.Render(widget, width: 60, height: 14);

            Assert.Contains("QR code needs 29 columns and 15 content rows", output);
            Assert.DoesNotContain('\u2588', output);
            Assert.DoesNotContain('\u2580', output);
        }

        [Fact]
        public void Too_narrow_viewport_also_shows_the_resize_message() {
            var widget = new QrCodeWidget(new bool[29, 29]);

            var output = TestSupport.Render(widget, width: 28, height: 30);

            Assert.Contains("QR code needs", output);
            Assert.DoesNotContain('\u2588', output);
        }

        [Fact]
        public void A_fitting_qr_slide_is_visible_without_scrolling() {
            var console = TestSupport.Console(80, 24);
            var modules = new bool[33, 33];
            modules[4, 4] = true;
            var slide = new Slide("Scan", new ISlideWidget[] { new QrCodeWidget(modules) });

            new Renderer(console).Render(new SlideShow(new[] { slide }));

            Assert.Contains('\u2580', console.Output);
            Assert.DoesNotContain("Scroll", console.Output);
            Assert.DoesNotContain("QR code needs", console.Output);
        }

        [Fact]
        public void A_qr_slide_never_shows_the_scroll_hint_when_the_code_is_too_tall() {
            var console = TestSupport.Console(80, 24);
            var widget = new QrCodeWidget(new bool[41, 41]);
            var slide = new Slide("Scan", new ISlideWidget[] { widget });

            new Renderer(console).Render(new SlideShow(new[] { slide }));

            Assert.Contains("QR code needs 41 columns and 21 content rows", console.Output);
            Assert.DoesNotContain("Scroll", console.Output);
        }

        private static List<string> RenderLines(QrCodeWidget widget, int width, int height) {
            var console = TestSupport.Console(width, height);
            var context = new WidgetRenderContext(console, width, height, SlideTheme.Default);
            return Lines(ContentCapture.Capture(console, widget.Render(context), width, null));
        }

        private static List<string> Lines(IEnumerable<SegmentLine> captured) =>
            captured
                .Where(line => line.Count > 0)
                .Select(line => string.Concat(line.Select(segment => segment.Text)))
                .ToList();
    }
}
