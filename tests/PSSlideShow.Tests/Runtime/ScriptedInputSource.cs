using System;
using System.Collections.Generic;
using PSSlideShow.Input;

namespace PSSlideShow.Tests {
    /// <summary>An <see cref="IInputSource"/> that replays a scripted key sequence, then quits.</summary>
    internal sealed class ScriptedInputSource : IInputSource {
        private readonly Queue<ConsoleKeyInfo> _keys;

        public ScriptedInputSource(params ConsoleKeyInfo[] keys) {
            _keys = new Queue<ConsoleKeyInfo>(keys);
        }

        public ConsoleKeyInfo ReadKey() => _keys.Count > 0 ? _keys.Dequeue() : Keys.Quit;

        // Scripted input is always immediately available, so the run loop never enters its
        // idle/resize-polling branch during tests.
        public bool KeyAvailable => true;
    }

    /// <summary>Convenience factory for <see cref="ConsoleKeyInfo"/> values used in tests.</summary>
    internal static class Keys {
        public static ConsoleKeyInfo Of(ConsoleKey key, char keyChar = '\0') =>
            new ConsoleKeyInfo(keyChar, key, shift: false, alt: false, control: false);

        public static readonly ConsoleKeyInfo Next = Of(ConsoleKey.N);
        public static readonly ConsoleKeyInfo Previous = Of(ConsoleKey.P);
        public static readonly ConsoleKeyInfo First = Of(ConsoleKey.Home);
        public static readonly ConsoleKeyInfo Last = Of(ConsoleKey.End);
        public static readonly ConsoleKeyInfo ScrollDown = Of(ConsoleKey.DownArrow);
        public static readonly ConsoleKeyInfo ScrollUp = Of(ConsoleKey.UpArrow);
        public static readonly ConsoleKeyInfo Enter = Of(ConsoleKey.Enter, '\r');
        public static readonly ConsoleKeyInfo Reset = Of(ConsoleKey.R);
        public static readonly ConsoleKeyInfo Quit = Of(ConsoleKey.Q);
    }
}
