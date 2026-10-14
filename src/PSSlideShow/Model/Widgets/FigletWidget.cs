using Spectre.Console;
using Spectre.Console.Rendering;
using PSSlideShow.Rendering;

namespace PSSlideShow.Model {
    /// <summary>Large ASCII banner text, rendered with a FIGlet font.</summary>
    public sealed class FigletWidget : SlideWidget {
        public FigletWidget(string text, Justify justify = Justify.Center, string? foreground = null, string? fontPath = null) {
            Text = text ?? string.Empty;
            Justify = justify;
            Foreground = foreground;
            FontPath = fontPath;
        }

        public string Text { get; }
        public Justify Justify { get; }
        public string? Foreground { get; }
        public string? FontPath { get; }
        public override bool IsRunnable => false;

        public override IRenderable Render(WidgetRenderContext context) {
            var figlet = string.IsNullOrEmpty(FontPath)
                ? new FigletText(Text)
                : new FigletText(FigletFont.Load(FontPath!), Text);

            figlet.Justification = Justify;
            if (!string.IsNullOrWhiteSpace(Foreground)) {
                figlet.Color = ColorResolver.Parse(Foreground!);
            }

            return figlet;
        }
    }
}
