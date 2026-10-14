using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;

namespace PSSlideShow.Model {
    /// <summary>
    /// A single labelled value used by <see cref="BarChartWidget"/> and
    /// <see cref="BreakdownChartWidget"/>. Items may be authored as hashtables
    /// (<c>@{ Label = 'A'; Value = 10; Color = 'Green' }</c>) or as objects with
    /// matching properties.
    /// </summary>
    public sealed class ChartItem {
        public ChartItem(string label, double value, string? color = null) {
            Label = label ?? string.Empty;
            Value = value;
            Color = color;
        }

        public string Label { get; }
        public double Value { get; }
        public string? Color { get; }

        /// <summary>Parses an arbitrary set of authoring objects into chart items.</summary>
        public static IReadOnlyList<ChartItem> ParseMany(IEnumerable<object?>? items) {
            var result = new List<ChartItem>();
            if (items == null) {
                return result;
            }

            foreach (var raw in items) {
                var parsed = ParseOne(raw);
                if (parsed != null) {
                    result.Add(parsed);
                }
            }

            return result;
        }

        private static ChartItem? ParseOne(object? raw) {
            if (raw == null) {
                return null;
            }

            if (raw is PSObject pso && pso.BaseObject is not PSCustomObject) {
                raw = pso.BaseObject;
            }

            if (raw is IDictionary dictionary) {
                var label = GetString(dictionary, "Label", "Name", "Key");
                var value = GetDouble(dictionary, "Value", "Count", "Amount");
                var color = GetString(dictionary, "Color", "Colour");
                return new ChartItem(label, value, color);
            }

            if (raw is PSObject wrapper) {
                var label = GetString(wrapper, "Label", "Name", "Key");
                var value = GetDouble(wrapper, "Value", "Count", "Amount");
                var color = GetString(wrapper, "Color", "Colour");
                return new ChartItem(label, value, color);
            }

            // Fall back to using the object's string form as a zero-value label.
            return new ChartItem(raw.ToString() ?? string.Empty, 0d);
        }

        private static string GetString(IDictionary dictionary, params string[] keys) {
            foreach (var key in keys) {
                foreach (DictionaryEntry entry in dictionary) {
                    if (string.Equals(entry.Key?.ToString(), key, StringComparison.OrdinalIgnoreCase)) {
                        return entry.Value?.ToString() ?? string.Empty;
                    }
                }
            }

            return string.Empty;
        }

        private static double GetDouble(IDictionary dictionary, params string[] keys) {
            foreach (var key in keys) {
                foreach (DictionaryEntry entry in dictionary) {
                    if (string.Equals(entry.Key?.ToString(), key, StringComparison.OrdinalIgnoreCase)) {
                        return ToDouble(entry.Value);
                    }
                }
            }

            return 0d;
        }

        private static string GetString(PSObject source, params string[] keys) {
            foreach (var key in keys) {
                var property = source.Properties[key];
                if (property?.Value != null) {
                    return property.Value.ToString() ?? string.Empty;
                }
            }

            return string.Empty;
        }

        private static double GetDouble(PSObject source, params string[] keys) {
            foreach (var key in keys) {
                var property = source.Properties[key];
                if (property?.Value != null) {
                    return ToDouble(property.Value);
                }
            }

            return 0d;
        }

        private static double ToDouble(object? value) {
            if (value == null) {
                return 0d;
            }

            if (value is PSObject pso) {
                value = pso.BaseObject;
            }

            try {
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException) {
                return 0d;
            }
        }
    }
}
