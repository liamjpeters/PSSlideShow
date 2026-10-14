namespace PSSlideShow.Model {
    /// <summary>
    /// Mutable runtime state for a runnable <see cref="CodeWidget"/>. The host/runner updates this as
    /// blocks are executed; the widget reads it when rendering its output region.
    /// </summary>
    public sealed class CodeExecutionState {
        /// <summary>Current execution status.</summary>
        public CodeRunStatus Status { get; set; } = CodeRunStatus.Next;

        /// <summary>Captured output text (may contain ANSI escape sequences) once the block has run.</summary>
        public string? Output { get; set; }

        /// <summary>Whether the captured output represents an error.</summary>
        public bool IsError { get; set; }

        /// <summary>Resets the state so the block can be run again from a clean slate.</summary>
        public void Reset() {
            Status = CodeRunStatus.Next;
            Output = null;
            IsError = false;
        }
    }
}
