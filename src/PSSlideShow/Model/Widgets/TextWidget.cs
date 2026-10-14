using Spectre.Console;
using Spectre.Console.Rendering;
using PSSlideShow.Rendering;

namespace PSSlideShow.Model {
    /// <summary>Prose/paragraph text. Supports Spectre console markup (e.g. <c>[bold]</c>, <c>[green]</c>).</summary>
    public sealed class TextWidget : SlideWidget {
        public TextWidget(string text, Justify justify = Justify.Left, string? foreground = null) {
            Text = text ?? string.Empty;
            Justify = justify;
            Foreground = foreground;
        }

        public string Text { get; }
        public Justify Justify { get; }
        public string? Foreground { get; }
        public override bool IsRunnable => false;

        public override IRenderable Render(WidgetRenderContext context) {
            var style = string.IsNullOrWhiteSpace(Foreground)
                ? Style.Plain
                : new Style(ColorResolver.Parse(Foreground!));
            var markup = new Markup(Text, style);
            return RenderUtil.Aligned(markup, Justify);
        }
    }
}
