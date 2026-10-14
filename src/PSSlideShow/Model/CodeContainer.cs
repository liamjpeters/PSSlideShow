namespace PSSlideShow.Model {
    /// <summary>Visual container style for a <see cref="CodeWidget"/>.</summary>
    public enum CodeContainer {
        /// <summary>Title / Output / closing rules only — no box.</summary>
        Simple,

        /// <summary>Code and output wrapped in panels with rounded corners.</summary>
        Rounded,

        /// <summary>Code and output wrapped in panels with square corners.</summary>
        Panel
    }

    /// <summary>Execution status of a runnable code block, used to render its output region.</summary>
    public enum CodeRunStatus {
        /// <summary>A runnable block that is not yet the next to execute.</summary>
        Waiting,

        /// <summary>The next block to execute (shows the run prompt).</summary>
        Next,

        /// <summary>Currently executing.</summary>
        Running,

        /// <summary>Finished; captured output is available.</summary>
        Ran
    }
}
