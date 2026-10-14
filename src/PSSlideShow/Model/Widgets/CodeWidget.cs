using System;
using System.Collections.Generic;
using System.Management.Automation;
using Spectre.Console;
using Spectre.Console.Rendering;
using PSSlideShow.Rendering;

namespace PSSlideShow.Model {
    /// <summary>
    /// A syntax-highlighted code block. PowerShell blocks are runnable by default; every other
    /// language is display-only. See <c>Get-Help New-SlideCode -Full</c> for its authoring options.
    /// </summary>
    public sealed class CodeWidget : SlideWidget {
        private IReadOnlyList<string>? _highlightedCache;

        public CodeWidget(
            string code,
            string language,
            string title,
            bool runnable,
            ScriptBlock? script = null,
            int startLineNumber = 1,
            bool hideLineNumbers = false,
            CodeContainer container = CodeContainer.Simple) {
            Code = code ?? string.Empty;
            Language = language ?? "text";
            Title = title ?? string.Empty;
            IsRunnable = runnable;
            Script = script;
            StartLineNumber = startLineNumber < 1 ? 1 : startLineNumber;
            HideLineNumbers = hideLineNumbers;
            Container = container;
        }

        public string Code { get; }
        public string Language { get; }
        public string Title { get; }
        public override bool IsRunnable { get; }

        /// <summary>The script to execute when runnable (PowerShell only); otherwise <see langword="null"/>.</summary>
        public ScriptBlock? Script { get; }

        public int StartLineNumber { get; }
        public bool HideLineNumbers { get; }
        public CodeContainer Container { get; }

        /// <summary>Live execution state (updated by the runner; read while rendering).</summary>
        public CodeExecutionState State { get; } = new CodeExecutionState();

        public override IRenderable Render(WidgetRenderContext context) {
            var codeBlock = BuildCodeBlock(context);

            switch (Container) {
                case CodeContainer.Rounded:
                case CodeContainer.Panel:
                    return BuildPanelled(context, codeBlock);
                default:
                    return BuildSimple(context, codeBlock);
            }
        }

        private IReadOnlyList<string> Highlighted =>
            _highlightedCache ??= SyntaxHighlighter.HighlightLines(Code, Language);

        private IRenderable BuildCodeBlock(WidgetRenderContext context) {
            var lines = Highlighted;
            if (HideLineNumbers) {
                var rows = new List<IRenderable>(lines.Count);
                foreach (var line in lines) rows.Add(new Markup(line.Length == 0 ? " " : line));
                return new Rows(rows);
            }

            var grid = new Grid();
            grid.AddColumn(new GridColumn().RightAligned().NoWrap().PadRight(1));
            grid.AddColumn(new GridColumn());

            for (var i = 0; i < lines.Count; i++) {
                var number = new Markup((StartLineNumber + i).ToString(), context.Theme.Chrome);
                var code = new Markup(lines[i].Length == 0 ? " " : lines[i]);
                grid.AddRow(number, code);
            }

            return grid;
        }

        private IRenderable BuildSimple(WidgetRenderContext context, IRenderable codeBlock) {
            var parts = new List<IRenderable>
            {
                new Rule(Title) { Justification = Justify.Left, Style = context.Theme.Accent },
                new Text(string.Empty),
                codeBlock,
                new Text(string.Empty)
            };

            if (IsRunnable) {
                parts.Add(new Rule("Output") { Justification = Justify.Left, Style = context.Theme.Output });
                parts.Add(BuildOutput(context));
                parts.Add(new Rule { Style = context.Theme.Output });
            }
            else {
                parts.Add(new Rule { Style = context.Theme.Accent });
            }

            return new Rows(parts);
        }

        private IRenderable BuildPanelled(WidgetRenderContext context, IRenderable codeBlock) {
            var border = Container == CodeContainer.Rounded ? BoxBorder.Rounded : BoxBorder.Square;

            var codePanel = new Panel(codeBlock) {
                Header = new PanelHeader(Title),
                Border = border,
                BorderStyle = context.Theme.Accent,
                Expand = true
            };

            if (!IsRunnable) {
                return codePanel;
            }

            var outputPanel = new Panel(BuildOutput(context)) {
                Header = new PanelHeader("Output"),
                Border = border,
                BorderStyle = context.Theme.Output,
                Expand = true
            };

            return new Rows(codePanel, outputPanel);
        }

        private IRenderable BuildOutput(WidgetRenderContext context) {
            switch (State.Status) {
                case CodeRunStatus.Ran:
                    var text = State.Output ?? string.Empty;
                    if (State.IsError) {
                        return new Markup(Markup.Escape(text), context.Theme.Error);
                    }

                    // Preserve any ANSI colour the script emitted (PowerShell 7 formats with $PSStyle) by
                    // parsing the escapes into styled Spectre segments, so colour is kept and width stays exact.
                    return new RenderedLines(AnsiTextParser.ToLines(text));
                case CodeRunStatus.Running:
                    return new Markup(Markup.Escape("Running..."), context.Theme.Chrome);
                case CodeRunStatus.Waiting:
                    return new Markup(Markup.Escape("Waiting..."), context.Theme.Chrome);
                case CodeRunStatus.Next:
                default:
                    return new Markup(Markup.Escape("Press [Enter] to run"), context.Theme.Chrome);
            }
        }
    }
}
