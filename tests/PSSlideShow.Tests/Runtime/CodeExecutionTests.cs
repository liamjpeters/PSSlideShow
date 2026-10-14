using PSSlideShow.Execution;
using PSSlideShow.Model;
using Xunit;

namespace PSSlideShow.Tests.Runtime {
    public class CodeExecutionTests {
        private static CodeWidget Code(string code) =>
            new CodeWidget(code, "powershell", "PowerShell", runnable: true,
                script: System.Management.Automation.ScriptBlock.Create(code));

        [Fact]
        public void Runs_code_and_captures_output() {
            using var runspaces = new RunspaceManager();
            var slide = new Slide("s", new ISlideWidget[] { });
            var widget = Code("1 + 1");
            var runspace = runspaces.GetRunspace(slide, widget);

            var result = new CodeRunner().Run(widget, runspace);

            Assert.False(result.IsError);
            Assert.Contains("2", result.Output);
        }

        [Fact]
        public void Captures_errors() {
            using var runspaces = new RunspaceManager();
            var slide = new Slide("s", new ISlideWidget[] { });
            var widget = Code("throw 'boom'");
            var runspace = runspaces.GetRunspace(slide, widget);

            var result = new CodeRunner().Run(widget, runspace);

            Assert.True(result.IsError);
            Assert.Contains("boom", result.Output);
        }

        [Fact]
        public void Slide_scope_shares_state_between_blocks() {
            using var runspaces = new RunspaceManager();
            var slide = new Slide("s", new ISlideWidget[] { }, CodeScope.Slide);
            var first = Code("$shared = 42");
            var second = Code("$shared");
            var runner = new CodeRunner();

            runner.Run(first, runspaces.GetRunspace(slide, first));
            var result = runner.Run(second, runspaces.GetRunspace(slide, second));

            Assert.Contains("42", result.Output);
        }

        [Fact]
        public void Block_scope_isolates_state() {
            using var runspaces = new RunspaceManager();
            var slide = new Slide("s", new ISlideWidget[] { }, CodeScope.Block);
            var first = Code("$isolated = 7");
            var second = Code("$null -eq $isolated");
            var runner = new CodeRunner();

            runner.Run(first, runspaces.GetRunspace(slide, first));
            var result = runner.Run(second, runspaces.GetRunspace(slide, second));

            Assert.Contains("True", result.Output);
        }
    }
}
