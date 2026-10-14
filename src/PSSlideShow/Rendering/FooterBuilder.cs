using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using Spectre.Console.Rendering;
using PSSlideShow.Input;

namespace PSSlideShow.Rendering {
    /// <summary>
    /// Builds the footer line: the active control hints on the left and the slide progress indicator
    /// on the right. Falls back to progress-only when the terminal is too narrow for the controls.
    /// </summary>
    public sealed class FooterBuilder {
        private readonly SlideTheme _theme;

        public FooterBuilder(SlideTheme? theme = null) {
            _theme = theme ?? SlideTheme.Default;
        }

        public IRenderable Build(SlideState state, int current, int total, int width) {
            var controls = string.Join("  ", KeyboardShortcuts.FooterControls(state));
            var progress = $"[{current:D2}/{total:D2}]";

            if (controls.Length + progress.Length + 1 > width) {
                controls = string.Empty;
            }

            var pad = width - controls.Length - progress.Length;
            if (pad < 0) {
                pad = 0;
            }

            var line = controls + new string(' ', pad) + progress;
            return new Text(line, _theme.Chrome);
        }
    }
}
