using System;
using System.Collections.Generic;
using System.Globalization;
using Spectre.Console;
using Spectre.Console.Rendering;
using PSSlideShow.Rendering;
using Calendar = Spectre.Console.Calendar;

namespace PSSlideShow.Model {
    /// <summary>A single highlighted day in a <see cref="CalendarWidget"/>.</summary>
    public sealed class CalendarEvent {
        public CalendarEvent(DateTime date, string? color = null) {
            Date = date;
            Color = color;
        }

        public DateTime Date { get; }
        public string? Color { get; }
    }

    /// <summary>A month calendar backed by Spectre's <see cref="Calendar"/>.</summary>
    public sealed class CalendarWidget : SlideWidget {
        public CalendarWidget(
            int year,
            int month,
            IReadOnlyList<CalendarEvent>? events = null,
            string? highlightColor = null,
            bool hideHeader = false,
            SlideBorder border = SlideBorder.Square,
            string? culture = null) {
            Year = year;
            Month = month;
            Events = events ?? new List<CalendarEvent>();
            HighlightColor = highlightColor;
            HideHeader = hideHeader;
            Border = border;
            Culture = culture;
        }

        public int Year { get; }
        public int Month { get; }
        public IReadOnlyList<CalendarEvent> Events { get; }
        public string? HighlightColor { get; }
        public bool HideHeader { get; }
        public SlideBorder Border { get; }
        public string? Culture { get; }
        public override bool IsRunnable => false;

        public override IRenderable Render(WidgetRenderContext context) {
            var calendar = new Calendar(Year, Month) {
                Border = BorderResolver.Resolve(Border),
                ShowHeader = !HideHeader
            };

            if (!string.IsNullOrWhiteSpace(Culture)) {
                calendar.Culture = CultureInfo.GetCultureInfo(Culture!);
            }

            var highlight = HighlightColor != null
                ? new Style(ColorResolver.Parse(HighlightColor))
                : context.Theme.Accent;
            calendar.HighlightStyle = highlight;

            foreach (var evt in Events) {
                calendar.AddCalendarEvent(evt.Date.Year, evt.Date.Month, evt.Date.Day);
            }

            return calendar;
        }
    }
}
