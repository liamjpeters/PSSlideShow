using System;
using System.Collections.Generic;

namespace PSSlideShow.Input {
    /// <summary>
    /// The single source of truth mapping keys to <see cref="ShortcutAction"/>s and describing the
    /// footer controls. Keeping both here ensures the input handler and the footer stay in sync.
    /// </summary>
    public static class KeyboardShortcuts {
        /// <summary>Maps a pressed key to its action (<see cref="ShortcutAction.None"/> if unbound).</summary>
        public static ShortcutAction Resolve(ConsoleKeyInfo key) {
            // if (key.Key == ConsoleKey.C && key.Modifiers.HasFlag(ConsoleModifiers.Control)) {
            //     return ShortcutAction.Quit;
            // }

            switch (key.Key) {
                case ConsoleKey.N:
                case ConsoleKey.RightArrow:
                case ConsoleKey.PageDown:
                case ConsoleKey.Spacebar:
                    return ShortcutAction.Next;
                case ConsoleKey.P:
                case ConsoleKey.LeftArrow:
                case ConsoleKey.PageUp:
                    return ShortcutAction.Previous;
                case ConsoleKey.Home:
                    return ShortcutAction.First;
                case ConsoleKey.End:
                    return ShortcutAction.Last;
                case ConsoleKey.UpArrow:
                    return ShortcutAction.ScrollUp;
                case ConsoleKey.DownArrow:
                    return ShortcutAction.ScrollDown;
                case ConsoleKey.Enter:
                    return ShortcutAction.Run;
                case ConsoleKey.R:
                    return ShortcutAction.Reset;
                case ConsoleKey.Q:
                case ConsoleKey.Escape:
                case ConsoleKey.C when key.Modifiers.HasFlag(ConsoleModifiers.Control):
                    return ShortcutAction.Quit;
                default:
                    return ShortcutAction.None;
            }
        }

        /// <summary>The footer control hints for the given slide state, in display order.</summary>
        public static IEnumerable<string> FooterControls(SlideState state) {
            if (state.HasPendingRun) {
                yield return "[Enter] Run";
            }

            if (!state.IsLast) {
                yield return "[N] Next";
            }

            if (!state.IsFirst) {
                yield return "[P] Prev";
            }

            if (state.HasRunnable) {
                yield return "[R] Reset";
            }

            yield return "[Q] Quit";

            if (state.Overflow) {
                yield return "[\u2191\u2193] Scroll";
            }
        }
    }
}
