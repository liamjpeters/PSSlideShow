using Spectre.Console;

namespace PSSlideShow.Rendering {
    /// <summary>
    /// Maps the module's semantic color roles to concrete Spectre styles in one place, so the
    /// palette can be tuned centrally (and, later, user-themed).
    /// </summary>
    /// <remarks>
    /// Colors are drawn from the terminal's own ANSI palette (plus the <c>dim</c> decoration) rather
    /// than hard-coded RGB, so the module inherits the active terminal/OS theme and never assumes a
    /// light or dark background.
    /// </remarks>
    public sealed class SlideTheme {
        /// <summary>The default, theme-aware palette.</summary>
        public static SlideTheme Default { get; } = new SlideTheme();

        /// <summary>Low-contrast chrome (titles, rules, footer, code placeholders).</summary>
        public Style Chrome { get; } = new Style(Color.Default, decoration: Decoration.Dim);

        /// <summary>Accent used for code title rules.</summary>
        public Style Accent { get; } = new Style(Color.Green);

        /// <summary>Rule/heading color for a code block's output region.</summary>
        public Style Output { get; } = new Style(Color.Blue);

        /// <summary>Error text (captured errors from runnable code).</summary>
        public Style Error { get; } = new Style(Color.Red);

        /// <summary>Full-contrast content default.</summary>
        public Style Content { get; } = Style.Plain;
    }
}
