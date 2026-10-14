namespace PSSlideShow.Model {
    /// <summary>
    /// Controls how runspaces are shared for runnable code blocks on a slide.
    /// </summary>
    public enum CodeScope {
        /// <summary>One runspace per slide; blocks on the same slide share state.</summary>
        Slide,

        /// <summary>A single runspace shared across the whole show.</summary>
        Show,

        /// <summary>Each runnable block gets its own isolated runspace.</summary>
        Block
    }
}
