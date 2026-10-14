using System;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using PSSlideShow.Model;

namespace PSSlideShow.Execution {
    /// <summary>The captured result of running a code block.</summary>
    public readonly struct CodeRunResult {
        public CodeRunResult(string output, bool isError) {
            Output = output ?? string.Empty;
            IsError = isError;
        }

        public string Output { get; }
        public bool IsError { get; }
    }

    /// <summary>
    /// Executes a <see cref="CodeWidget"/>'s script in a resolved runspace and captures its output
    /// (formatted like the default host, with ANSI colours preserved) and any errors. PowerShell 7 emits
    /// ANSI-coloured formatting (e.g. from <c>Format-Table</c> via <c>$PSStyle</c>); those escape
    /// sequences are kept in the captured string and parsed into styled Spectre segments at render time
    /// (see <c>AnsiTextParser</c>), so the colour survives without breaking width accounting.
    /// </summary>
    public sealed class CodeRunner {
        private readonly int _outputWidth;

        public CodeRunner(int outputWidth = 120) {
            _outputWidth = outputWidth < 20 ? 20 : outputWidth;
        }

        public CodeRunResult Run(CodeWidget widget, Runspace runspace) {
            if (widget == null) throw new ArgumentNullException(nameof(widget));
            if (runspace == null) throw new ArgumentNullException(nameof(runspace));

            using var powerShell = PowerShell.Create();
            powerShell.Runspace = runspace;

            var code = widget.Script?.ToString() ?? widget.Code;
            powerShell.AddScript(code);
            powerShell.AddCommand("Out-String").AddParameter("Width", _outputWidth);

            try {
                var results = powerShell.Invoke();
                var builder = new StringBuilder();
                foreach (var item in results) {
                    builder.Append(item?.ToString());
                }

                var output = builder.ToString().TrimEnd('\r', '\n');

                if (powerShell.HadErrors && powerShell.Streams.Error.Count > 0) {
                    var errors = string.Join(
                        Environment.NewLine,
                        powerShell.Streams.Error.Select(e => e.ToString()));
                    var text = output.Length > 0
                        ? output + Environment.NewLine + errors
                        : errors;
                    return new CodeRunResult(text, true);
                }

                return new CodeRunResult(output, false);
            }
            catch (RuntimeException ex) {
                return new CodeRunResult(ex.Message, true);
            }
        }
    }
}
