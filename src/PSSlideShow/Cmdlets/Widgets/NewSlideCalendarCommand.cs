using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using PSSlideShow.Model;

namespace PSSlideShow.Cmdlets.Widgets {
    /// <summary>
    /// <para type="synopsis">Adds a month calendar to a slide.</para>
    /// <para type="description">
    /// Highlight days with <c>-Event</c>, which accepts <see cref="DateTime"/> values, day numbers
    /// (interpreted against <c>-Year</c>/<c>-Month</c>), or hashtables with a <c>Date</c> key.
    /// </para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideCalendar")]
    [OutputType(typeof(CalendarWidget))]
    public sealed class NewSlideCalendarCommand : WidgetCommand {
        /// <summary>Year to display. Defaults to the current year.</summary>
        [Parameter]
        public int Year { get; set; } = DateTime.Now.Year;

        /// <summary>Month to display (1-12). Defaults to the current month.</summary>
        [Parameter]
        public int Month { get; set; } = DateTime.Now.Month;

        /// <summary>Days to highlight (DateTime, day number, or hashtable with a Date key).</summary>
        [Parameter]
        public object[]? Event { get; set; }

        /// <summary>Highlight color name/hex. Defaults to the accent style.</summary>
        [Parameter]
        public string? HighlightColor { get; set; }

        /// <summary>Hides the month/year header.</summary>
        [Parameter]
        public SwitchParameter HideHeader { get; set; }

        /// <summary>Border style. Default <c>Square</c>.</summary>
        [Parameter]
        public SlideBorder Border { get; set; } = SlideBorder.Square;

        /// <summary>Culture name (e.g. <c>en-US</c>) controlling day/month names.</summary>
        [Parameter]
        public string? Culture { get; set; }

        protected override void EndProcessing() {
            var events = new List<CalendarEvent>();
            if (Event != null) {
                foreach (var raw in Event) {
                    var parsed = ParseEvent(raw);
                    if (parsed != null) {
                        events.Add(parsed);
                    }
                }
            }

            WriteWidget(new CalendarWidget(
                Year,
                Month,
                events,
                HighlightColor,
                HideHeader.IsPresent,
                Border,
                Culture));
        }

        private CalendarEvent? ParseEvent(object? raw) {
            if (raw == null) {
                return null;
            }

            if (raw is PSObject pso) {
                raw = pso.BaseObject;
            }

            switch (raw) {
                case DateTime dt:
                    return new CalendarEvent(dt);
                case int day:
                    return new CalendarEvent(new DateTime(Year, Month, day));
                case IDictionary dictionary: {
                        var date = ResolveDate(dictionary);
                        if (date == null) {
                            return null;
                        }

                        var color = GetString(dictionary, "Color", "Colour");
                        return new CalendarEvent(date.Value, string.IsNullOrEmpty(color) ? null : color);
                    }
                case string s when DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed):
                    return new CalendarEvent(parsed);
                default:
                    return null;
            }
        }

        private DateTime? ResolveDate(IDictionary dictionary) {
            foreach (DictionaryEntry entry in dictionary) {
                if (!string.Equals(entry.Key?.ToString(), "Date", StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                var value = entry.Value is PSObject wrapper ? wrapper.BaseObject : entry.Value;
                return value switch {
                    DateTime dt => dt,
                    int day => new DateTime(Year, Month, day),
                    string s when DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed) => parsed,
                    _ => (DateTime?)null
                };
            }

            return null;
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
    }
}
