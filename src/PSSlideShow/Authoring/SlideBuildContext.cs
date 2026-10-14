using System;
using System.Collections.Generic;

namespace PSSlideShow.Authoring {
    /// <summary>
    /// A thread-scoped stack that tracks the current authoring scope while <c>New-Slide</c> and
    /// container widgets invoke their body script blocks. It lets nested cmdlets (for example
    /// <c>New-SlideTreeNode</c> or <c>New-SlideColumn</c>) validate that they are used inside a valid
    /// parent and discover the nearest enclosing container.
    /// </summary>
    /// <remarks>
    /// Top-level widgets are still collected by capturing the output of the invoked script block;
    /// this context only carries the <em>scope</em> information used for validation and nesting, so
    /// the whole DSL stays declarative.
    /// </remarks>
    public static class SlideBuildContext {
        [ThreadStatic]
        private static Stack<string>? _scopes;

        private static Stack<string> Scopes => _scopes ??= new Stack<string>();

        /// <summary>The innermost active scope name, or <see langword="null"/> if none.</summary>
        public static string? Current => Scopes.Count > 0 ? Scopes.Peek() : null;

        /// <summary>Whether any active scope matches <paramref name="name"/>.</summary>
        public static bool InScope(string name) => Scopes.Contains(name);

        /// <summary>Pushes a scope for the lifetime of the returned token.</summary>
        public static IDisposable Push(string name) {
            Scopes.Push(name);
            return new Popper();
        }

        private sealed class Popper : IDisposable {
            private bool _done;

            public void Dispose() {
                if (_done) return;
                _done = true;
                if (Scopes.Count > 0) Scopes.Pop();
            }
        }
    }
}
