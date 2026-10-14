using PSSlideShow.Model;
using Xunit;

namespace PSSlideShow.Tests.Widgets {
    public class AlertWidgetTests {
        [Fact]
        public void Renders_title_and_body() {
            var output = TestSupport.Render(new AlertWidget(AlertSeverity.Note, "Body text here", "Heads up"));
            Assert.Contains("Heads up", output);
            Assert.Contains("Body text here", output);
        }

        [Fact]
        public void Defaults_title_to_the_severity_name() {
            var output = TestSupport.Render(new AlertWidget(AlertSeverity.Warning, "Careful"));
            Assert.Contains("Warning", output);
        }

        [Fact]
        public void Shows_a_left_bar() {
            var output = TestSupport.Render(new AlertWidget(AlertSeverity.Caution, "Risky"));
            Assert.Contains("\u258c", output); // ▌ left bar glyph
        }

        [Fact]
        public void Shows_the_severity_symbol_by_default() {
            var output = TestSupport.Render(new AlertWidget(AlertSeverity.Tip, "Do this"));
            Assert.Contains("\u2605", output); // ★
        }

        [Fact]
        public void NoSymbol_hides_the_symbol() {
            var output = TestSupport.Render(new AlertWidget(AlertSeverity.Tip, "Do this", showSymbol: false));
            Assert.DoesNotContain("\u2605", output);
            Assert.Contains("Tip", output);
        }

        [Fact]
        public void Renders_markup_in_the_body() {
            var output = TestSupport.Render(new AlertWidget(AlertSeverity.Important, "[bold]Bold[/] point"));
            Assert.Contains("Bold point", output);
            Assert.DoesNotContain("[bold]", output);
        }

        [Fact]
        public void Is_not_runnable() {
            Assert.False(new AlertWidget(AlertSeverity.Note, "x").IsRunnable);
        }
    }
}
