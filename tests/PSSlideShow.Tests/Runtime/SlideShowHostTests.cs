using PSSlideShow.Execution;
using PSSlideShow.Hosting;
using PSSlideShow.Input;
using PSSlideShow.Model;
using PSSlideShow.Rendering;
using Spectre.Console.Testing;
using Xunit;

namespace PSSlideShow.Tests.Runtime {
    public class SlideShowHostTests {
        private static SlideShowHost Host(TestConsole console, IInputSource input) =>
            new SlideShowHost(
                console,
                input,
                new Renderer(console),
                new RunspaceManager(),
                new CodeRunner(),
                alternateScreen: false);

        private static Slide TextSlide(string title) =>
            new Slide(title, new ISlideWidget[] { new TextWidget(title + " body") });

        [Fact]
        public void Next_advances_the_current_slide() {
            var console = TestSupport.Console(80, 20);
            var show = new SlideShow(new[] { TextSlide("One"), TextSlide("Two"), TextSlide("Three") });
            var input = new ScriptedInputSource(Keys.Next, Keys.Next, Keys.Quit);

            Host(console, input).Run(show);

            Assert.Equal(2, show.CurrentIndex);
        }

        [Fact]
        public void Previous_and_home_navigate_back() {
            var console = TestSupport.Console(80, 20);
            var show = new SlideShow(new[] { TextSlide("One"), TextSlide("Two"), TextSlide("Three") });
            var input = new ScriptedInputSource(Keys.Last, Keys.Previous, Keys.First, Keys.Quit);

            Host(console, input).Run(show);

            Assert.Equal(0, show.CurrentIndex);
        }

        [Fact]
        public void Enter_runs_the_next_code_block_and_captures_output() {
            var console = TestSupport.Console(100, 30);
            var code = new CodeWidget("'hello-from-run'", "powershell", "PowerShell", runnable: true,
                script: System.Management.Automation.ScriptBlock.Create("'hello-from-run'"));
            var slide = new Slide("Demo", new ISlideWidget[] { code });
            var show = new SlideShow(new[] { slide });
            var input = new ScriptedInputSource(Keys.Enter, Keys.Quit);

            Host(console, input).Run(show);

            Assert.Equal(CodeRunStatus.Ran, code.State.Status);
            Assert.Contains("hello-from-run", code.State.Output);
            Assert.False(code.State.IsError);
        }

        [Fact]
        public void Reset_rewinds_code_block_state() {
            var console = TestSupport.Console(100, 30);
            var code = new CodeWidget("'x'", "powershell", "PowerShell", runnable: true,
                script: System.Management.Automation.ScriptBlock.Create("'x'"));
            var slide = new Slide("Demo", new ISlideWidget[] { code });
            var show = new SlideShow(new[] { slide });
            var input = new ScriptedInputSource(Keys.Enter, Keys.Reset, Keys.Quit);

            Host(console, input).Run(show);

            Assert.Equal(CodeRunStatus.Next, code.State.Status);
            Assert.True(string.IsNullOrEmpty(code.State.Output));
        }
    }
}
