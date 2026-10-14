using Spectre.Console.Rendering;

namespace PSSlideShow.Model {
    /// <summary>
    /// A renderable unit of slide content. Every widget cmdlet (<c>New-Slide*</c>) produces one of
    /// these, and <c>New-Slide</c> collects them, in order, as the slide's content stack.
    /// </summary>
    /// <remarks>
    /// Implementing this interface is the primary extension point for the module. A new display-only
    /// widget needs nothing from the engine beyond a <see cref="Render"/> that returns a Spectre
    /// <see cref="IRenderable"/>; the renderer, scrolling, and footer require no changes.
    /// </remarks>
    public interface ISlideWidget {
        /// <summary>
        /// Builds the Spectre renderable for this widget using the supplied layout constraints and
        /// theme. Called every frame, so implementations should be side-effect free.
        /// </summary>
        IRenderable Render(WidgetRenderContext context);

        /// <summary>
        /// Whether this widget participates in the <c>[Enter] Run</c> flow. Display-only widgets
        /// return <see langword="false"/>.
        /// </summary>
        bool IsRunnable { get; }
    }
}
