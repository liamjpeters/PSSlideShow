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
            Assert.Equal(22, almostWide.Count);
            Assert.Equal(29, wide.Count);
            Assert.Equal("\u2588\u2588", wide[4].Substring(8, 2));
            Assert.Equal(small, RenderLines(widget, 29, 15));
        }

        [Fact]
        public void Centers_the_qr_vertically_without_scaling_its_modules() {
            var modules = new bool[29, 29];
            modules[4, 4] = true;
            var widget = new QrCodeWidget(modules);
            var console = TestSupport.Console(29, 29);
            var context = new WidgetRenderContext(console, 29, 29, SlideTheme.Default);

            var captured = ContentCapture.Capture(console, widget.Render(context), 29, null);
            var lines = Lines(captured);

            Assert.Equal(22, lines.Count);
            Assert.Equal('\u2580', lines[9][4]);
            Assert.Equal(Style.Plain, captured[0].Single().Style);
            Assert.Equal(Color.White, captured[7].Single().Style.Background);
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
        public void A_qr_code_can_share_a_flow_column_with_explanatory_text() {
            var console = TestSupport.Console(100, 24);
            var modules = new bool[33, 33];
            modules[4, 4] = true;
            var left = new LayoutRegion(new ISlideWidget[] { new TextWidget("Please give me feedback") });
            var right = new LayoutRegion(new ISlideWidget[] { new QrCodeWidget(modules) });
            var layout = new LayoutWidget(LayoutOrientation.Columns, new[] { left, right }) {
                VerticalPadding = 0
            };
            var slide = new Slide("Feedback", new ISlideWidget[] { layout });

            new Renderer(console).Render(new SlideShow(new[] { slide }));

            Assert.Contains("Please give me feedback", console.Output);
            Assert.Contains('\u2580', console.Output);
            Assert.DoesNotContain("QR code needs", console.Output);
        }

        [Fact]
        public void A_stacked_qr_can_be_scrolled_completely_into_view() {
            var console = TestSupport.Console(80, 24);
            var text = new TextWidget(string.Join("\n", Enumerable.Range(1, 8).Select(i => $"Line {i}"))) {
                VerticalPadding = 0
            };
            var modules = new bool[33, 33];
            modules[4, 4] = true;
            modules[28, 24] = true;
            var show = new SlideShow(new[] {
                new Slide("Feedback", new ISlideWidget[] { text, new QrCodeWidget(modules) })
            }) {
                ScrollOffset = int.MaxValue
            };

            new Renderer(console).Render(show);

            Assert.True(show.ScrollOffset > 0);
            Assert.Contains("Scroll", console.Output);
            Assert.Equal(2, console.Output.Count(c => c == '\u2580'));
            Assert.DoesNotContain("QR code needs", console.Output);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void A_narrow_column_shows_a_resize_message_instead_of_a_partial_qr(bool fill) {
            var console = TestSupport.Console(80, 24);
            var modules = new bool[33, 33];
            modules[4, 24] = true;
            var left = new LayoutRegion(new ISlideWidget[] { new TextWidget("Feedback please") }, size: 60);
            var right = new LayoutRegion(new ISlideWidget[] { new QrCodeWidget(modules) });
            var layout = new LayoutWidget(LayoutOrientation.Columns, new[] { left, right }, fill) {
                VerticalPadding = 0
            };

            new Renderer(console).Render(new SlideShow(new[] { new Slide("Scan", new ISlideWidget[] { layout }) }));

            Assert.Contains("QR code needs", console.Output);
            Assert.DoesNotContain('\u2580', console.Output);
        }

        [Fact]
        public void A_fill_column_that_fits_keeps_both_edges_of_the_qr_visible() {
            var console = TestSupport.Console(100, 24);
            var modules = new bool[33, 33];
            modules[4, 4] = true;
            modules[4, 24] = true;
            var left = new LayoutRegion(new ISlideWidget[] { new TextWidget("Feedback please") });
            var right = new LayoutRegion(new ISlideWidget[] { new QrCodeWidget(modules) });
            var layout = new LayoutWidget(LayoutOrientation.Columns, new[] { left, right }, fill: true) {
                VerticalPadding = 0
            };

            new Renderer(console).Render(new SlideShow(new[] { new Slide("Scan", new ISlideWidget[] { layout }) }));

            Assert.Equal(2, console.Output.Count(c => c == '\u2580'));
            Assert.DoesNotContain("QR code needs", console.Output);
        }

        [Fact]
        public void A_short_fill_row_shows_a_resize_message_instead_of_clipping_the_qr() {
            var console = TestSupport.Console(80, 24);
            var top = new LayoutRegion(new ISlideWidget[] { new TextWidget("Feedback please") }, size: 12);
            var bottom = new LayoutRegion(new ISlideWidget[] { new QrCodeWidget(new bool[33, 33]) });
            var layout = new LayoutWidget(LayoutOrientation.Rows, new[] { top, bottom }, fill: true) {
                VerticalPadding = 0
            };

            new Renderer(console).Render(new SlideShow(new[] { new Slide("Scan", new ISlideWidget[] { layout }) }));

            Assert.Contains("QR code needs", console.Output);
            Assert.DoesNotContain('\u2580', console.Output);
        }

        [Fact]
        public void A_fill_region_passes_its_actual_dimensions_to_inner_renderables() {
            var console = TestSupport.Console(76, 19);
            var probe = new SizeProbeWidget();
            var left = new LayoutRegion(new ISlideWidget[] { new TextWidget("Feedback please") }, size: 60);
            var right = new LayoutRegion(new ISlideWidget[] { probe });
            var layout = new LayoutWidget(LayoutOrientation.Columns, new[] { left, right }, fill: true);
            var context = new WidgetRenderContext(console, 76, 19, SlideTheme.Default);

            ContentCapture.Capture(console, layout.Render(context), 76, 19);

            Assert.InRange(probe.Width, 1, 16);
            Assert.InRange(probe.Height.GetValueOrDefault(), 1, 19);
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

        private sealed class SizeProbeWidget : SlideWidget {
            public int Width { get; private set; }
            public int? Height { get; private set; }

            public override IRenderable Render(WidgetRenderContext context) => new SizeProbe(this);

            private sealed class SizeProbe : IRenderable {
                private readonly SizeProbeWidget _widget;

                public SizeProbe(SizeProbeWidget widget) => _widget = widget;

                public Measurement Measure(RenderOptions options, int maxWidth) =>
                    new Measurement(1, maxWidth);

                public IEnumerable<Segment> Render(RenderOptions options, int maxWidth) {
                    _widget.Width = maxWidth;
                    _widget.Height = options.Height;
                    yield return new Segment("X");
                }
            }
        }
    }
}
