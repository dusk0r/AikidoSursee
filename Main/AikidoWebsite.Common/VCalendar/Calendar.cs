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
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("PRODID:-//AmigoOnline//NONSGML AikidoWebsite//DE");

            foreach (var calendarEvent in Events.Where(e => e.Summary != null)) {
                sb.Append(calendarEvent.ToString());
            }

            sb.AppendLine("END:VCALENDAR");

            return sb.ToString();
        }
    }
}
