using System;
using System.Collections.Generic;

namespace PSSlideShow.Model {
    /// <summary>
    /// The mutable state of a running show: the ordered slides, which one is current, and the
    /// per-slide scroll offsets. Navigation methods clamp to valid bounds and report whether the
    /// current slide actually changed.
    /// </summary>
    public sealed class SlideShow {
        private readonly List<Slide> _slides;
        private readonly List<int> _scrollOffsets;

        public SlideShow(IReadOnlyList<Slide> slides, int startIndex = 0) {
            if (slides == null) throw new ArgumentNullException(nameof(slides));
            _slides = new List<Slide>(slides);
            _scrollOffsets = new List<int>(new int[_slides.Count]);
            CurrentIndex = Clamp(startIndex);
        }

        public IReadOnlyList<Slide> Slides => _slides;
        public int CurrentIndex { get; private set; }
        public int Count => _slides.Count;
        public Slide? Current => _slides.Count == 0 ? null : _slides[CurrentIndex];
        public bool IsFirst => CurrentIndex == 0;
        public bool IsLast => CurrentIndex == _slides.Count - 1;

        public bool Next() => GoTo(CurrentIndex + 1);
        public bool Previous() => GoTo(CurrentIndex - 1);
        public bool First() => GoTo(0);
        public bool Last() => GoTo(_slides.Count - 1);

        /// <summary>Navigates to the (clamped) index; returns whether the current slide changed.</summary>
        public bool GoTo(int index) {
            if (_slides.Count == 0) return false;
            var clamped = Clamp(index);
            if (clamped == CurrentIndex) return false;
            CurrentIndex = clamped;
            return true;
        }

        public int GetScrollOffset(int index) => _scrollOffsets[index];

        public void SetScrollOffset(int index, int value) =>
            _scrollOffsets[index] = value < 0 ? 0 : value;

        /// <summary>The scroll offset of the current slide.</summary>
        public int ScrollOffset {
            get => _slides.Count == 0 ? 0 : _scrollOffsets[CurrentIndex];
            set { if (_slides.Count != 0) SetScrollOffset(CurrentIndex, value); }
        }

        private int Clamp(int index) =>
            _slides.Count == 0 ? 0 : Math.Max(0, Math.Min(index, _slides.Count - 1));
    }
}
