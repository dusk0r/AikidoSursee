using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Common.VCalendar
{

    public class Calendar {
        public IList<CalendarEvent> Events { get; private set; } = new List<CalendarEvent>();

        public Calendar() {
        }

        public bool IsValid => Events.All(e => e.IsValid);

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("BEGIN:VCALENDAR\r\n");
            sb.Append("VERSION:2.0\r\n");
            sb.Append("CALSCALE:GREGORIAN\r\n");
            sb.Append("PRODID:-//AmigoOnline//NONSGML AikidoWebsite//DE\r\n");

            foreach (var calendarEvent in Events.Where(e => e.Summary != null)) {
                sb.Append(calendarEvent.ToString());
            }

            sb.Append("END:VCALENDAR\r\n");

            return sb.ToString();
        }
    }
}
