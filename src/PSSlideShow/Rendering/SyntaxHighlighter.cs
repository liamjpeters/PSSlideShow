using System;
using System.Collections.Generic;
using System.Text;
using Spectre.Console;
using TextMateSharp.Grammars;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

namespace PSSlideShow.Rendering {
    /// <summary>
    /// Turns source code into per-line Spectre markup using TextMate grammars (via TextMateSharp).
    /// The registry, theme, and grammars are cached; if a grammar for the requested language is not
    /// available the code is returned escaped but unstyled.
    /// </summary>
    public static class SyntaxHighlighter {
        private static readonly object Sync = new object();
        private static ThemeName _themeName = ThemeName.DarkPlus;
        private static RegistryOptions? _options;
        private static Registry? _registry;
        private static Theme? _theme;
        private static readonly Dictionary<string, IGrammar?> Grammars =
            new Dictionary<string, IGrammar?>(StringComparer.OrdinalIgnoreCase);

        private static void EnsureInit() {
            if (_registry != null) return;
            lock (Sync) {
                if (_registry != null) return;
                _options = new RegistryOptions(_themeName);
                _registry = new Registry(_options);
                _theme = _registry.GetTheme();
            }
        }

        /// <summary>
        /// Selects the color palette used for highlighting. Call this once before rendering. Passing
        /// <see langword="true"/> chooses a light-background palette (LightPlus); otherwise a
        /// dark-background palette (DarkPlus). TextMate themes use absolute RGB colors, so unlike the
        /// rest of the module's ANSI-palette chrome the highlighter cannot inherit the terminal theme
        /// automatically — it has to be told which one to use.
        /// </summary>
        public static void UseTheme(bool light) {
            var desired = light ? ThemeName.HighContrastLight : ThemeName.DarkPlus;
            lock (Sync) {
                if (_registry != null && _themeName == desired) return;
                _themeName = desired;
                _options = new RegistryOptions(desired);
                _registry = new Registry(_options);
                _theme = _registry.GetTheme();
                // Grammars are loaded from the registry, so drop the cache when it is rebuilt.
                Grammars.Clear();
            }
        }

        private static IGrammar? GetGrammar(string languageId) {
            EnsureInit();
            lock (Sync) {
                if (Grammars.TryGetValue(languageId, out var cached)) return cached;

                IGrammar? grammar = null;
                try {
                    var scope = _options!.GetScopeByLanguageId(languageId);
                    if (!string.IsNullOrEmpty(scope)) {
                        grammar = _registry!.LoadGrammar(scope);
                    }
                }
                catch {
                    grammar = null;
                }

                Grammars[languageId] = grammar;
                return grammar;
            }
        }

        /// <summary>Highlights <paramref name="code"/>, returning one Spectre markup string per line.</summary>
        public static IReadOnlyList<string> HighlightLines(string code, string? languageId) {
            var rawLines = TextUtil.SplitLines(code);
            var grammar = string.IsNullOrWhiteSpace(languageId) ? null : GetGrammar(languageId!);
            if (grammar == null) {
                var escaped = new string[rawLines.Length];
                for (var i = 0; i < rawLines.Length; i++) escaped[i] = Markup.Escape(rawLines[i]);
                return escaped;
            }

            var result = new List<string>(rawLines.Length);
            IStateStack? ruleStack = null;
            // TextMateSharp's tokenizer drives the native Onig regex engine, which keeps shared
            // mutable state and is not thread-safe. Serialize tokenization (and the theme lookups it
            // feeds) so concurrent renders cannot corrupt that native state.
            lock (Sync) {
                foreach (var line in rawLines) {
                    var tokenizeResult = grammar.TokenizeLine(line, ruleStack, TimeSpan.MaxValue);
                    ruleStack = tokenizeResult.RuleStack;

                    var builder = new StringBuilder();
                    foreach (var token in tokenizeResult.Tokens) {
                        var start = Math.Min(token.StartIndex, line.Length);
                        var end = Math.Min(token.EndIndex, line.Length);
                        if (end <= start) continue;

                        var text = Markup.Escape(line.Substring(start, end - start));
                        var hex = ForegroundHex(token.Scopes);
                        if (hex != null) {
                            builder.Append('[').Append(hex).Append(']').Append(text).Append("[/]");
                        }
                        else {
                            builder.Append(text);
                        }
                    }

                    result.Add(builder.ToString());
                }
            }

            return result;
        }

        private static string? ForegroundHex(List<string> scopes) {
            var rules = _theme!.Match(scopes);
            if (rules == null) return null;
            foreach (var rule in rules) {
                if (rule.foreground > 0) {
                    var color = _theme.GetColor(rule.foreground);
                    if (!string.IsNullOrEmpty(color)) return color;
                }
            }

            return null;
        }
    }
}
