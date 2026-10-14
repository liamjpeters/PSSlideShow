using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;

namespace PSSlideShow.Authoring {
    /// <summary>Adapts a value-formatter script block (which receives the value as <c>$_</c>) to a managed delegate.</summary>
    public static class ScriptBlockFormatter {
        public static Func<double, string>? ToFunc(ScriptBlock? script) {
            if (script == null) {
                return null;
            }

            return value => {
                var variables = new List<PSVariable>
                {
                    new PSVariable("_", value),
                    new PSVariable("PSItem", value)
                };

                var results = script.InvokeWithContext(null, variables, value);
                if (results.Count > 0) {
                    return results[results.Count - 1]?.ToString() ?? string.Empty;
                }

                return value.ToString(CultureInfo.InvariantCulture);
            };
        }
    }
}
