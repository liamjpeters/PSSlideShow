using System;

namespace PSSlideShow.Input {
    /// <summary>Abstracts key input so the run loop can be exercised without a real terminal.</summary>
    public interface IInputSource {
        /// <summary>Blocks until a key is available and returns it.</summary>
        ConsoleKeyInfo ReadKey();

        /// <summary>
        /// True when a key can be read without blocking. The run loop polls this while idle so it can
        /// react to terminal resizes; non-interactive sources should simply return <c>true</c>.
        /// </summary>
        bool KeyAvailable { get; }
    }

    /// <summary>Production input source backed by the system console.</summary>
    public sealed class ConsoleInputSource : IInputSource {
        public ConsoleKeyInfo ReadKey() => Console.ReadKey(intercept: true);

        public bool KeyAvailable => Console.KeyAvailable;
    }
}
