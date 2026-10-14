using PSSlideShow.Model;
using Xunit;

namespace PSSlideShow.Tests.Widgets {
    public class CodeWidgetTests {
        private static CodeWidget Code(
            string code = "aaa\nbbb",
            bool runnable = false,
            int startLine = 1,
            bool hideLineNumbers = false,
            CodeContainer container = CodeContainer.Simple)
            => new CodeWidget(code, "powershell", "PowerShell", runnable, null, startLine, hideLineNumbers, container);

        [Fact]
        public void Renders_code_and_line_numbers() {
            var output = TestSupport.Render(Code(startLine: 1));
            Assert.Contains("aaa", output);
            Assert.Contains("bbb", output);
            Assert.Contains("1", output);
            Assert.Contains("2", output);
            Assert.Contains("PowerShell", output);
        }

        [Fact]
        public void Line_numbers_honor_start_line() {
            var output = TestSupport.Render(Code(startLine: 10));
            Assert.Contains("10", output);
            Assert.Contains("11", output);
        }

        [Fact]
        public void Hidden_line_numbers_omit_the_gutter() {
            var output = TestSupport.Render(Code(startLine: 10, hideLineNumbers: true));
            Assert.DoesNotContain("10", output);
            Assert.Contains("aaa", output);
        }

        [Fact]
        public void Runnable_shows_output_section_with_prompt() {
            var output = TestSupport.Render(Code(runnable: true));
            Assert.Contains("Output", output);
            Assert.Contains("Press [Enter] to run", output);
        }

        [Fact]
        public void Non_runnable_has_no_output_section() {
            var output = TestSupport.Render(Code(runnable: false));
            Assert.DoesNotContain("Output", output);
            Assert.DoesNotContain("Press [Enter] to run", output);
        }

        [Fact]
        public void Ran_state_shows_captured_output() {
            var widget = Code(runnable: true);
            widget.State.Status = CodeRunStatus.Ran;
            widget.State.Output = "hello-world";
            var output = TestSupport.Render(widget);
            Assert.Contains("hello-world", output);
        }

        [Fact]
        public void Panel_container_renders_title_and_code() {
            var output = TestSupport.Render(Code(container: CodeContainer.Panel));
            Assert.Contains("PowerShell", output);
            Assert.Contains("aaa", output);
        }
    }
}
