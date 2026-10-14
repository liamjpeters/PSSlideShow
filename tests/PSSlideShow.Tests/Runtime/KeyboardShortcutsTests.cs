using System;
using PSSlideShow.Input;
using PSSlideShow.Rendering;
using Xunit;

namespace PSSlideShow.Tests.Runtime {
    public class KeyboardShortcutsTests {
        [Theory]
        [InlineData(ConsoleKey.N, ShortcutAction.Next)]
        [InlineData(ConsoleKey.RightArrow, ShortcutAction.Next)]
        [InlineData(ConsoleKey.PageDown, ShortcutAction.Next)]
        [InlineData(ConsoleKey.Spacebar, ShortcutAction.Next)]
        [InlineData(ConsoleKey.P, ShortcutAction.Previous)]
        [InlineData(ConsoleKey.LeftArrow, ShortcutAction.Previous)]
        [InlineData(ConsoleKey.Home, ShortcutAction.First)]
        [InlineData(ConsoleKey.End, ShortcutAction.Last)]
        [InlineData(ConsoleKey.UpArrow, ShortcutAction.ScrollUp)]
        [InlineData(ConsoleKey.DownArrow, ShortcutAction.ScrollDown)]
        [InlineData(ConsoleKey.Enter, ShortcutAction.Run)]
        [InlineData(ConsoleKey.R, ShortcutAction.Reset)]
        [InlineData(ConsoleKey.Q, ShortcutAction.Quit)]
        [InlineData(ConsoleKey.Escape, ShortcutAction.Quit)]
        [InlineData(ConsoleKey.A, ShortcutAction.None)]
        [InlineData(ConsoleKey.C, ShortcutAction.None)]
        public void Resolves_unmodified_keys_to_actions(ConsoleKey key, ShortcutAction expected) {
            var info = new ConsoleKeyInfo('\0', key, false, false, false);
            Assert.Equal(expected, KeyboardShortcuts.Resolve(info));
        }

        [Fact]
        public void Resolves_ctrl_c_to_quit() {
            var info = new ConsoleKeyInfo('\u0003', ConsoleKey.C, false, false, true);

            Assert.Equal(ShortcutAction.Quit, KeyboardShortcuts.Resolve(info));
        }

        [Fact]
        public void Footer_hides_prev_on_first_and_next_on_last() {
            var first = new SlideState(isFirst: true, isLast: false, hasRunnable: false, hasPendingRun: false, overflow: false);
            var last = new SlideState(isFirst: false, isLast: true, hasRunnable: false, hasPendingRun: false, overflow: false);

            Assert.DoesNotContain("[P] Prev", KeyboardShortcuts.FooterControls(first));
            Assert.Contains("[N] Next", KeyboardShortcuts.FooterControls(first));
            Assert.DoesNotContain("[N] Next", KeyboardShortcuts.FooterControls(last));
            Assert.Contains("[P] Prev", KeyboardShortcuts.FooterControls(last));
        }

        [Fact]
        public void Footer_shows_run_reset_and_scroll_conditionally() {
            var state = new SlideState(isFirst: false, isLast: false, hasRunnable: true, hasPendingRun: true, overflow: true);
            var controls = string.Join(" ", KeyboardShortcuts.FooterControls(state));

            Assert.Contains("[Enter] Run", controls);
            Assert.Contains("[R] Reset", controls);
            Assert.Contains("Scroll", controls);
        }
    }

    public class FooterBuilderTests {
        [Fact]
        public void Includes_controls_and_progress() {
            var builder = new FooterBuilder();
            var state = new SlideState(isFirst: true, isLast: false, hasRunnable: false, hasPendingRun: false, overflow: false);
            var output = TestSupport.RenderRaw(builder.Build(state, 1, 3, 80));

            Assert.Contains("[N] Next", output);
            Assert.Contains("[01/03]", output);
        }

        [Fact]
        public void Narrow_width_falls_back_to_progress_only() {
            var builder = new FooterBuilder();
            var state = new SlideState(isFirst: false, isLast: false, hasRunnable: true, hasPendingRun: true, overflow: true);
            var output = TestSupport.RenderRaw(builder.Build(state, 5, 14, 12));

            Assert.Contains("[05/14]", output);
            Assert.DoesNotContain("[N] Next", output);
        }
    }
}
