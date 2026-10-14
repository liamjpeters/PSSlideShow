using Spectre.Console;
using Spectre.Console.Rendering;
using PSSlideShow.Rendering;

namespace PSSlideShow.Model {
    /// <summary>The severity of an <see cref="AlertWidget"/>, mirroring GitHub's Markdown alert types.</summary>
    public enum AlertSeverity {
        /// <summary>Useful information (blue).</summary>
        Note,

        /// <summary>Helpful advice (green).</summary>
        Tip,

        /// <summary>Key information the audience needs (purple).</summary>
        Important,

        /// <summary>Urgent information needing attention (amber).</summary>
        Warning,

        /// <summary>Risks or negative outcomes (red).</summary>
        Caution,
    }

    /// <summary>
    /// A GitHub-style alert callout: a thick coloured bar down the left edge, a coloured (optionally
    /// symbol-prefixed) title, and body text below it. The colour and symbol are chosen from the
    /// <see cref="AlertSeverity"/>. The body text supports Spectre console markup.
    /// </summary>
    public sealed class AlertWidget : SlideWidget {
        public AlertWidget(AlertSeverity severity, string text, string? title = null, bool showSymbol = true) {
            Severity = severity;
            Text = text ?? string.Empty;
            Title = title;
            ShowSymbol = showSymbol;
        }

        public AlertSeverity Severity { get; }
        public string Text { get; }
        public string? Title { get; }
        public bool ShowSymbol { get; }

        /// <summary>The heading shown for the alert: the supplied title, or the severity name by default.</summary>
        public string Heading => string.IsNullOrWhiteSpace(Title) ? Severity.ToString() : Title!;

        public override IRenderable Render(WidgetRenderContext context) {
            var color = ColorFor(Severity);
            var headingText = ShowSymbol
                ? $"{SymbolFor(Severity)} {Markup.Escape(Heading)}"
                : Markup.Escape(Heading);

            var heading = new Markup(headingText, new Style(color, decoration: Decoration.Bold));
            var body = new Markup(Text, context.Theme.Content);
            var content = new Rows(heading, new Text(string.Empty), body);

            return new Panel(content) {
                Border = new LeftBarBorder(),
                BorderStyle = new Style(color),
                Padding = new Padding(1, 0, 1, 0),
                Expand = false,
            };
        }

        internal static Color ColorFor(AlertSeverity severity) => severity switch {
            AlertSeverity.Note => Color.Blue,
            AlertSeverity.Tip => Color.Green,
            AlertSeverity.Important => Color.Purple,
            AlertSeverity.Warning => Color.Orange1,
            AlertSeverity.Caution => Color.Red,
            _ => Color.Default,
        };

        internal static string SymbolFor(AlertSeverity severity) => severity switch {
            // Text-presentation glyphs (no emoji variant) so they inherit the title colour and stay a
            // single cell wide. Emoji glyphs (e.g. U+2757, U+26A0) render coloured and double-width,
            // which both overrides the severity colour and eats the space before the title.
            AlertSeverity.Note => "\u24d8",      // ⓘ circled i
            AlertSeverity.Tip => "\u2605",       // ★ black star
            AlertSeverity.Important => "\u2762",  // ❢ heavy exclamation ornament
            AlertSeverity.Warning => "\u25b2",   // ▲ black up-pointing triangle
            AlertSeverity.Caution => "\u2298",   // ⊘ circled division slash
            _ => string.Empty,
        };
    }
}
