using System;
using System.Collections.Generic;
using System.Linq;

namespace PSSlideShow.Model {
    /// <summary>
    /// A single slide: a title, an ordered stack of widgets, and the runspace-sharing policy for
    /// any runnable code it contains.
    /// </summary>
    public sealed class Slide {
        public Slide(string? title, IReadOnlyList<ISlideWidget> widgets, CodeScope codeScope = CodeScope.Slide) {
            Title = title;
            Widgets = widgets ?? throw new ArgumentNullException(nameof(widgets));
            CodeScope = codeScope;
        }

        /// <summary>Optional title text. <see langword="null"/>/empty reserves a blank title row.</summary>
        public string? Title { get; }

        /// <summary>The slide's content, in emission order.</summary>
        public IReadOnlyList<ISlideWidget> Widgets { get; }

        /// <summary>Runspace-sharing policy for runnable code on this slide.</summary>
        public CodeScope CodeScope { get; }

        /// <summary>Top-level runnable widgets, in order (containers are traversed by the runner).</summary>
        public IEnumerable<ISlideWidget> RunnableWidgets => Widgets.Where(w => w.IsRunnable);

        /// <summary>Whether this slide has any runnable code block.</summary>
        public bool HasRunnable => Widgets.Any(w => w.IsRunnable);
    }
}
