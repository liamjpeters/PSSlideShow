namespace PSSlideShow.Input {
    /// <summary>
    /// A snapshot of the current slide's state, used to decide which footer controls are visible and
    /// which shortcuts are active.
    /// </summary>
    public readonly struct SlideState {
        public SlideState(bool isFirst, bool isLast, bool hasRunnable, bool hasPendingRun, bool overflow) {
            IsFirst = isFirst;
            IsLast = isLast;
            HasRunnable = hasRunnable;
            HasPendingRun = hasPendingRun;
            Overflow = overflow;
        }

        /// <summary>True when the current slide is the first in the deck.</summary>
        public bool IsFirst { get; }

        /// <summary>True when the current slide is the last in the deck.</summary>
        public bool IsLast { get; }

        /// <summary>True when the slide contains at least one runnable code block.</summary>
        public bool HasRunnable { get; }

        /// <summary>True when a runnable block is still waiting to be run.</summary>
        public bool HasPendingRun { get; }

        /// <summary>True when the content overflows the viewport (scrolling is possible).</summary>
        public bool Overflow { get; }
    }
}
