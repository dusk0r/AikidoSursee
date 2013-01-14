using AikidoWebsite.Common.VCalendar;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;

namespace AikidoWebsite.Common.UnitTests.VCalendar {

    [TestFixture]
    public class CalendarTests {

        [Test]
        public void CanCreateCalendar() {
            var calendar = new Calendar();
            calendar.Events.Add(new CalendarEvent {
                UID = Guid.NewGuid().ToString(),
                Organizer = new Organizer("Peter Gut", "peter.gut@test.com"),
                Timestamp = DateTime.Today,
                Starttime = new DateTime(2012, 12, 22, 14, 0, 0),
                Endtime = new DateTime(2012, 12, 22, 23, 59, 0),
                Summary = "Hello"
            });

            calendar.IsValid.Should().BeTrue();

            var ical = calendar.ToString();
            ical.Should().NotBeNull().And.NotBeBlank();
            ical.Should().StartWith("BEGIN:VCALENDAR");
            ical.Should().EndWith("END:VCALENDAR" + Environment.NewLine);
            ical.Should().Contain("Peter Gut");
            ical.Should().Contain("20121222T130000Z");  // UTC
            ical.Should().Contain("Hello");
        }
    }
}
