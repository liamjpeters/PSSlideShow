using System.Collections.Generic;
using PSSlideShow.Model;
using Xunit;

namespace PSSlideShow.Tests.Widgets {
    public class CalendarWidgetTests {
        [Fact]
        public void Renders_month_header_and_days() {
            var output = TestSupport.Render(
                new CalendarWidget(2024, 1, culture: "en-US"), width: 40, height: 20);
            Assert.Contains("2024", output);
            Assert.Contains("January", output);
            Assert.Contains("15", output);
        }

        [Fact]
        public void Highlights_event_days() {
            var events = new List<CalendarEvent> { new CalendarEvent(new System.DateTime(2024, 1, 10)) };
            var output = TestSupport.Render(
                new CalendarWidget(2024, 1, events, "green", culture: "en-US"), width: 40, height: 20);
            Assert.Contains("10", output);
        }
    }
}
