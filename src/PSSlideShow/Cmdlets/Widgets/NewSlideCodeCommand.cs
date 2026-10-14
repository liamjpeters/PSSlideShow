using System.IO;
using System.Linq;
using System.Management.Automation;
using PSSlideShow.Model;
using PSSlideShow.Rendering;

namespace PSSlideShow.Cmdlets.Widgets {
    /// <summary>
    /// <para type="synopsis">Adds a syntax-highlighted code block to a slide.</para>
    /// <para type="description">
    /// PowerShell blocks are runnable by default (pass <c>-NonRunnable</c> to disable); other
    /// languages are display-only. Supply the code via <c>-ScriptBlock</c> (default), <c>-Code</c>,
    /// or <c>-FilePath</c>.
    /// </para>
    /// <para type="description">
    /// In the <c>-ScriptBlock</c> set, <c>-Setup</c> and <c>-Cleanup</c> run around the visible block
    /// (before and after) without being displayed &#8212; handy for preparing and tidying demo state. In
    /// the <c>-FilePath</c> set, <c>-StartLine</c>/<c>-EndLine</c> choose the displayed slice while
    /// <c>-RunFromLine</c>/<c>-RunToLine</c> choose a (usually wider) slice to execute; the displayed
    /// lines must fall within the executed lines. All four line numbers are optional.
    /// </para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideCode", DefaultParameterSetName = ScriptBlockSet)]
    [OutputType(typeof(CodeWidget))]
    public sealed class NewSlideCodeCommand : WidgetCommand {
        private const string ScriptBlockSet = "ScriptBlock";
        private const string CodeSet = "Code";
        private const string FilePathSet = "FilePath";

        /// <summary>Literal PowerShell to display (and, unless <c>-NonRunnable</c>, execute).</summary>
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = ScriptBlockSet)]
        public ScriptBlock? ScriptBlock { get; set; }

        /// <summary>Script run before the visible block (not displayed). Use it to prepare demo state.</summary>
        [Parameter(ParameterSetName = ScriptBlockSet)]
        public ScriptBlock? Setup { get; set; }

        /// <summary>Script run after the visible block, in a <c>finally</c> (not displayed). Use it to tidy up.</summary>
        [Parameter(ParameterSetName = ScriptBlockSet)]
        public ScriptBlock? Cleanup { get; set; }

        /// <summary>Literal code as text (use a here-string). The route for non-PowerShell inline code.</summary>
        [Parameter(Mandatory = true, ParameterSetName = CodeSet)]
        public string? Code { get; set; }

        /// <summary>Path to a file to read the code from.</summary>
        [Parameter(Mandatory = true, ParameterSetName = FilePathSet)]
        public string? FilePath { get; set; }

        /// <summary>First line of the file to display (1-based). Line numbering starts here.</summary>
        [Parameter(ParameterSetName = FilePathSet)]
        public int StartLine { get; set; }

        /// <summary>Last line of the file to display (inclusive).</summary>
        [Parameter(ParameterSetName = FilePathSet)]
        public int EndLine { get; set; }

        /// <summary>First line to execute (1-based). Defaults to <c>-StartLine</c>; must be &#8804; <c>-StartLine</c>.</summary>
        [Parameter(ParameterSetName = FilePathSet)]
        public int RunFromLine { get; set; }

        /// <summary>Last line to execute (inclusive). Defaults to <c>-EndLine</c>; must be &#8805; <c>-EndLine</c>.</summary>
        [Parameter(ParameterSetName = FilePathSet)]
        public int RunToLine { get; set; }

        /// <summary>Grammar id for highlighting; determines the title and runnability. Default <c>powershell</c>.</summary>
        [Parameter]
        public string Language { get; set; } = "powershell";

        /// <summary>Marks a PowerShell block display-only (no effect for other languages).</summary>
        [Parameter]
        public SwitchParameter NonRunnable { get; set; }

        /// <summary>Hide the line-number gutter.</summary>
        [Parameter]
        public SwitchParameter HideLineNumbers { get; set; }

        /// <summary>Visual container style.</summary>
        [Parameter]
        public CodeContainer Container { get; set; } = CodeContainer.Simple;

        protected override void EndProcessing() {
            string rawCode;
            string? runCode = null;
            string? fileName = null;
            var startLineNumber = 1;
            ScriptBlock? script = null;

            switch (ParameterSetName) {
                case CodeSet:
                    rawCode = TextUtil.Dedent(Code);
                    runCode = rawCode;
                    break;

                case FilePathSet:
                    var resolved = GetUnresolvedProviderPathFromPSPath(FilePath);
                    var allLines = File.ReadAllLines(resolved);

                    // Displayed slice (StartLine..EndLine). Both are optional: an absent start implies the
                    // first line, an absent end implies the last line.
                    var displayStart = System.Math.Max(1, StartLine > 0 ? StartLine : 1);
                    var displayEnd = System.Math.Min(allLines.Length, EndLine > 0 ? EndLine : allLines.Length);

                    // Executed slice (RunFromLine..RunToLine). Defaults to the displayed slice, so execution
                    // matches display unless explicitly widened.
                    var runFrom = System.Math.Max(1, RunFromLine > 0 ? RunFromLine : displayStart);
                    var runTo = System.Math.Min(allLines.Length, RunToLine > 0 ? RunToLine : displayEnd);

                    // The displayed lines must fall within the executed lines.
                    if (displayStart < runFrom || displayEnd > runTo) {
                        ThrowTerminatingError(new ErrorRecord(
                            new PSArgumentException(
                                $"The displayed line range ({displayStart}-{displayEnd}) must fall within the " +
                                $"execution range ({runFrom}-{runTo}). Adjust -StartLine/-EndLine or " +
                                "-RunFromLine/-RunToLine so the displayed lines are contained within the executed lines."),
                            "DisplayRangeOutsideRunRange",
                            ErrorCategory.InvalidArgument,
                            FilePath));
                        return;
                    }

                    rawCode = TextUtil.Dedent(Slice(allLines, displayStart, displayEnd));
                    runCode = TextUtil.Dedent(Slice(allLines, runFrom, runTo));
                    fileName = Path.GetFileName(resolved);
                    startLineNumber = displayStart;
                    break;

                case ScriptBlockSet:
                default:
                    rawCode = TextUtil.Dedent(ScriptBlock!.ToString());
                    script = ComposeRunScript(ScriptBlock!, Setup, Cleanup);
                    break;
            }

            var isPowerShell = Language.Equals("powershell", System.StringComparison.OrdinalIgnoreCase)
                || Language.Equals("ps1", System.StringComparison.OrdinalIgnoreCase);
            var runnable = isPowerShell && !NonRunnable.IsPresent;

            if (runnable && script == null && !string.IsNullOrWhiteSpace(runCode)) {
                script = System.Management.Automation.ScriptBlock.Create(runCode);
            }

            var title = CodeTitle.Derive(Language, fileName);

            WriteWidget(new CodeWidget(
                rawCode,
                Language,
                title,
                runnable,
                script,
                startLineNumber,
                HideLineNumbers.IsPresent,
                Container));
        }

        private static string Slice(string[] lines, int start, int end) =>
            start <= end
                ? string.Join("\n", lines.Skip(start - 1).Take(end - start + 1))
                : string.Empty;

        /// <summary>
        /// Combines the visible block with the invisible <c>-Setup</c>/<c>-Cleanup</c> blocks into a
        /// single script to execute. Setup runs first; the main block runs inside a <c>try</c> whose
        /// <c>finally</c> runs cleanup, so cleanup happens even if the main block throws.
        /// </summary>
        private static ScriptBlock ComposeRunScript(ScriptBlock main, ScriptBlock? setup, ScriptBlock? cleanup) {
            if (setup == null && cleanup == null) {
                return main;
            }

            var builder = new System.Text.StringBuilder();
            if (setup != null) {
                builder.AppendLine(setup.ToString());
            }

            if (cleanup != null) {
                builder.AppendLine("try {");
                builder.AppendLine(main.ToString());
                builder.AppendLine("} finally {");
                builder.AppendLine(cleanup.ToString());
                builder.AppendLine("}");
            }
            else {
                builder.AppendLine(main.ToString());
            }

            return ScriptBlock.Create(builder.ToString());
        }
    }
}
