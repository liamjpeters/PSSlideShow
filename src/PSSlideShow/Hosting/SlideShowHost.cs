using System;
using System.Linq;
using System.Threading;
using Spectre.Console;
using PSSlideShow.Execution;
using PSSlideShow.Input;
using PSSlideShow.Model;
using PSSlideShow.Rendering;

namespace PSSlideShow.Hosting {
    /// <summary>
    /// The interactive run loop: reads keys from an <see cref="IInputSource"/>, dispatches
    /// navigation / scroll / run / reset actions, and re-renders through the injected console. Manages
    /// the alternate-screen buffer and cursor visibility.
    /// </summary>
    public sealed class SlideShowHost {
        private const int ResizePollMilliseconds = 50;

        private readonly IAnsiConsole _console;
        private readonly IInputSource _input;
        private readonly Renderer _renderer;
        private readonly RunspaceManager _runspaces;
        private readonly CodeRunner _runner;
        private readonly DynamicUpdateManager _dynamicUpdates;
        private readonly bool _alternateScreen;

        public SlideShowHost(
            IAnsiConsole console,
            IInputSource input,
            Renderer renderer,
            RunspaceManager runspaces,
            CodeRunner runner,
            bool alternateScreen = true,
            DynamicUpdateManager? dynamicUpdates = null) {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            _runspaces = runspaces ?? throw new ArgumentNullException(nameof(runspaces));
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
            _dynamicUpdates = dynamicUpdates ?? new DynamicUpdateManager(_runspaces);
            _alternateScreen = alternateScreen;
        }

        /// <summary>Runs the presentation until the user quits. Blocks the calling thread.</summary>
        public void Run(SlideShow show) {
            if (show == null) throw new ArgumentNullException(nameof(show));

            Enter();
            try {
                ActivateSlide(show.Current);
                RenderFrame(show);

                while (show.Count > 0) {
                    var action = KeyboardShortcuts.Resolve(WaitForKey(show));
                    ProcessDynamicUpdates(show);
                    if (action == ShortcutAction.Quit) {
                        break;
                    }

                    Dispatch(action, show);
                    RenderFrame(show);
                }
            }
            finally {
                _dynamicUpdates.Dispose();
                Exit();
                _runspaces.Dispose();
            }
        }

        private void Dispatch(ShortcutAction action, SlideShow show) {
            switch (action) {
                case ShortcutAction.Next:
                    if (show.Next()) ActivateSlide(show.Current);
                    break;
                case ShortcutAction.Previous:
                    if (show.Previous()) ActivateSlide(show.Current);
                    break;
                case ShortcutAction.First:
                    if (show.First()) ActivateSlide(show.Current);
                    break;
                case ShortcutAction.Last:
                    if (show.Last()) ActivateSlide(show.Current);
                    break;
                case ShortcutAction.ScrollUp:
                    show.ScrollOffset = Math.Max(0, show.ScrollOffset - 1);
                    break;
                case ShortcutAction.ScrollDown:
                    show.ScrollOffset += 1;
                    break;
                case ShortcutAction.Run:
                    RunNext(show.Current);
                    break;
                case ShortcutAction.Reset:
                    ResetSlide(show.Current);
                    break;
            }
        }

        private void ActivateSlide(Slide? slide) {
            PrepareSlide(slide);
            if (slide != null) {
                _dynamicUpdates.Activate(slide);
            }
        }

        private void ProcessDynamicUpdates(SlideShow show) {
            if (_dynamicUpdates.ProcessCompleted()) {
                RenderFrame(show);
            }
        }

        /// <summary>Sets the sequential-run status of a slide's runnable blocks (next / waiting).</summary>
        private void PrepareSlide(Slide? slide) {
            if (slide == null) {
                return;
            }

            var pendingSeen = false;
            foreach (var block in RunnableCollector.Collect(slide)) {
                if (block.State.Status == CodeRunStatus.Ran) {
                    continue;
                }

                block.State.Status = pendingSeen ? CodeRunStatus.Waiting : CodeRunStatus.Next;
                pendingSeen = true;
            }
        }

        private void RunNext(Slide? slide) {
            if (slide == null) {
                return;
            }

            var next = RunnableCollector.Collect(slide)
                .FirstOrDefault(b => b.State.Status != CodeRunStatus.Ran);
            if (next == null) {
                return;
            }

            next.State.Status = CodeRunStatus.Running;
            var runspace = _runspaces.GetRunspace(slide, next);
            var result = _runner.Run(next, runspace);

            next.State.Status = CodeRunStatus.Ran;
            next.State.Output = result.Output;
            next.State.IsError = result.IsError;

            PrepareSlide(slide);
        }

        private void ResetSlide(Slide? slide) {
            if (slide == null) {
                return;
            }

            _runspaces.ResetSlide(slide);
            foreach (var block in RunnableCollector.Collect(slide)) {
                block.State.Reset();
            }

            PrepareSlide(slide);
        }

        private void RenderFrame(SlideShow show) {
            // Home the cursor and repaint over the previous frame instead of clearing the whole screen.
            // A full clear (ESC[2J) blanks the screen for an instant before the new frame lands, which
            // reads as a flicker on every keypress. The renderer pads every cell of the viewport, so the
            // old frame is fully overwritten in place with no blank interval.
            _console.Cursor.SetPosition(1, 1);
            _renderer.Render(show);
        }

        /// <summary>
        /// Blocks until a key is pressed, re-rendering whenever the terminal is resized while idle so
        /// the slide always fits the current window.
        /// </summary>
        private ConsoleKeyInfo WaitForKey(SlideShow show) {
            var size = TryGetConsoleSize();
            while (!_input.KeyAvailable) {
                ProcessDynamicUpdates(show);
                Thread.Sleep(ResizePollMilliseconds);
                var current = TryGetConsoleSize();
                if (current.HasValue && current != size) {
                    size = current;
                    RenderFrame(show);
                }
            }

            return _input.ReadKey();
        }

        private static (int Width, int Height)? TryGetConsoleSize() {
            try {
                return (Console.WindowWidth, Console.WindowHeight);
            }
            catch {
                // No real console attached (e.g. output redirected) — resize tracking is unavailable.
                return null;
            }
        }

        private void Enter() {
            if (_alternateScreen) {
                WriteAnsi("\u001b[?1049h");
            }

            // A single clear on entry so the first in-place repaint starts from a known-blank buffer.
            _console.Clear();
            _console.Cursor.Hide();
        }

        private void Exit() {
            _console.Cursor.Show();
            if (_alternateScreen) {
                WriteAnsi("\u001b[?1049l");
            }
        }

        private void WriteAnsi(string sequence) {
            _console.Profile.Out.Writer.Write(sequence);
            _console.Profile.Out.Writer.Flush();
        }
    }
}
