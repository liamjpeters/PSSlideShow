using Spectre.Console.Rendering;

namespace PSSlideShow.Model {
    /// <summary>
    /// Base class for the built-in slide widgets. Adds optional outer padding that the renderer applies
    /// around each <em>top-level</em> widget (the widgets collected directly by <c>New-Slide</c>), giving
    /// consistent visual separation out of the box. Nested content — layout region bodies, tree nodes —
    /// is composed inside its parent widget and is not padded here.
    /// </summary>
    /// <remarks>
    /// New widgets should inherit this class rather than implementing <see cref="ISlideWidget"/> directly,
    /// so they pick up padding (and any future common behaviour) for free. Implement <see cref="Render"/>;
    /// override <see cref="IsRunnable"/> only if the widget participates in the <c>[Enter] Run</c> flow.
    /// </remarks>
    public abstract class SlideWidget : ISlideWidget {
        /// <summary>Left/right padding in cells applied around this widget. Defaults to 0.</summary>
        public int HorizontalPadding { get; set; }

        /// <summary>Top/bottom padding in rows applied around this widget. Defaults to 1.</summary>
        public int VerticalPadding { get; set; } = 1;

        /// <inheritdoc />
        public abstract IRenderable Render(WidgetRenderContext context);

        /// <inheritdoc />
        public virtual bool IsRunnable => false;
    }
}
