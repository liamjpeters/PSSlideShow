using Spectre.Console;
using Spectre.Console.Rendering;

namespace PSSlideShow.Model {
    /// <summary>
    /// A pixel canvas. Painting happens at authoring time (inside <c>New-SlideCanvas -Paint</c>); this
    /// widget just holds the already-painted <see cref="SlideCanvas"/> and applies scaling on render.
    /// </summary>
    public sealed class CanvasWidget : SlideWidget {
        private readonly SlideCanvas _canvas;

        public CanvasWidget(SlideCanvas canvas, int? maxWidth = null, bool noScale = false) {
            _canvas = canvas;
            MaxWidth = maxWidth;
            NoScale = noScale;
        }

        public int Width => _canvas.Width;
        public int Height => _canvas.Height;
        public int? MaxWidth { get; }
        public bool NoScale { get; }
        public override bool IsRunnable => false;

        public override IRenderable Render(WidgetRenderContext context) {
            var canvas = _canvas.Inner;
            canvas.Scale = !NoScale;
            canvas.MaxWidth = MaxWidth ?? context.Width;
            return canvas;
        }
    }
}
