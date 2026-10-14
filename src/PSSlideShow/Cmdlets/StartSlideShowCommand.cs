using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Spectre.Console;
using PSSlideShow.Execution;
using PSSlideShow.Hosting;
using PSSlideShow.Input;
using PSSlideShow.Model;
using PSSlideShow.Rendering;

namespace PSSlideShow.Cmdlets {
    /// <summary>
    /// <para type="synopsis">Launches the interactive slide show.</para>
    /// <para type="description">
    /// Presents the supplied slides full-screen and keyboard-driven, blocking until the user quits.
    /// Provide slides via <c>-Slides</c> (or the pipeline) or load a deck file with <c>-Path</c>.
    /// Press <c>q</c>, <c>Esc</c>, or <c>Ctrl+C</c> to quit.
    /// </para>
    /// </summary>
    /// <example>
    ///   <code>
    ///   Start-SlideShow -Path ./deck.ps1
    ///   ./deck.ps1 | Start-SlideShow
    ///   </code>
    /// </example>
    [Cmdlet(VerbsLifecycle.Start, "SlideShow", DefaultParameterSetName = SlidesSet)]
    public sealed class StartSlideShowCommand : PSCmdlet {
        private const string SlidesSet = "Slides";
        private const string PathSet = "Path";

        /// <summary>Slides to present. Accepts pipeline input.</summary>
        [Parameter(ParameterSetName = SlidesSet, ValueFromPipeline = true, Position = 0)]
        public Slide[]? Slides { get; set; }

        /// <summary>Path to a <c>.ps1</c> deck whose emitted slides are collected and presented.</summary>
        [Parameter(ParameterSetName = PathSet, Mandatory = true)]
        public string? Path { get; set; }

        /// <summary>1-based index of the slide to open on. Default 1.</summary>
        [Parameter]
        public int StartAt { get; set; } = 1;

        /// <summary>Render inline instead of switching to the alternate screen buffer.</summary>
        [Parameter]
        public SwitchParameter NoAlternateScreen { get; set; }

        /// <summary>
        /// Left/right gutter (in cells) that insets the slide content from the terminal edges. The title,
        /// rules and footer stay full-width. Default 2; set 0 to run content flush to the edge.
        /// </summary>
        [Parameter]
        public int ContentPadding { get; set; } = 2;

        /// <summary>
        /// Palette used to syntax-highlight code blocks. <c>Auto</c> (default) detects the terminal
        /// background and picks a light or dark palette; <c>Dark</c> or <c>Light</c> forces one. Use an
        /// explicit value if automatic detection guesses wrong for your terminal.
        /// </summary>
        [Parameter]
        public SyntaxThemePreference SyntaxTheme { get; set; } = SyntaxThemePreference.Auto;

        private readonly List<Slide> _slides = new();

        protected override void ProcessRecord() {
            if (Slides != null) {
                foreach (var slide in Slides) {
                    if (slide != null) {
                        _slides.Add(slide);
                    }
                }
            }
        }

        protected override void EndProcessing() {
            if (ParameterSetName == PathSet) {
                LoadFromPath();
            }

            if (_slides.Count == 0) {
                WriteWarning("No slides to present.");
                return;
            }

            var show = new SlideShow(_slides, StartAt - 1);

            // Ensure non-ASCII glyphs (bullets, box-drawing, tree guides) render correctly, and bind
            // the Spectre console to the UTF-8 writer we just installed.
            var previousEncoding = System.Console.OutputEncoding;
            try {
                System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            }
            catch {
                // Some hosts disallow changing the encoding; fall back to whatever is configured.
            }

            // Pick the syntax-highlighting palette before rendering. Detection (Auto) queries the
            // terminal, so it has to happen before the host switches to the alternate screen buffer.
            var useLightSyntax = SyntaxTheme switch {
                SyntaxThemePreference.Light => true,
                SyntaxThemePreference.Dark => false,
                _ => TerminalBackground.IsLight() ?? false
            };
            SyntaxHighlighter.UseTheme(useLightSyntax);

            var console = AnsiConsole.Create(new AnsiConsoleSettings {
                Ansi = AnsiSupport.Detect,
                ColorSystem = ColorSystemSupport.Detect,
                Out = new AnsiConsoleOutput(System.Console.Out)
            });
            var renderer = new Renderer(console, contentPadding: ContentPadding);
            var runspaces = new RunspaceManager();
            var runner = new CodeRunner();
            var input = new ConsoleInputSource();

            var host = new SlideShowHost(
                console,
                input,
                renderer,
                runspaces,
                runner,
                alternateScreen: !NoAlternateScreen.IsPresent);

            var previousTreatControlCAsInput = System.Console.TreatControlCAsInput;
            try {
                System.Console.TreatControlCAsInput = true;
                host.Run(show);
            }
            finally {
                try { System.Console.TreatControlCAsInput = previousTreatControlCAsInput; }
                catch { /* best-effort restore */ }
                try { System.Console.OutputEncoding = previousEncoding; }
                catch { /* best-effort restore */ }
            }
        }

        private void LoadFromPath() {
            var resolved = GetUnresolvedProviderPathFromPSPath(Path);
            if (!File.Exists(resolved)) {
                ThrowTerminatingError(new ErrorRecord(
                    new FileNotFoundException("Deck file not found.", resolved),
                    "DeckNotFound",
                    ErrorCategory.ObjectNotFound,
                    resolved));
            }

            var script = ScriptBlock.Create("& $args[0]");
            foreach (var result in script.Invoke(resolved)) {
                if (result?.BaseObject is Slide slide) {
                    _slides.Add(slide);
                }
            }
        }
    }
}
