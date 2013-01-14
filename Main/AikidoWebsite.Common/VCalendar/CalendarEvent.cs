using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AikidoWebsite.Common;

namespace AikidoWebsite.Common.VCalendar {
    
    public class CalendarEvent {
        private static readonly int Summary_Limit = 100;

        public string UID { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime Starttime { get; set; }
        public DateTime Endtime { get; set; }
        public Organizer Organizer { get; set; }
        public string Summary { get; set; }

        public bool IsValid {
            get {
                try {
                    Check();
                    return true;
                } catch (CalendarDataException e) {
                    return false;
                }
            }
        }

        public void Check() {
            if (String.IsNullOrWhiteSpace(UID)) {
                throw new CalendarDataException(this, "Keine UID");
            }

            if (Organizer == null) {
                throw new CalendarDataException(this, "Kein Organizer");
            }

            if (!Organizer.IsValid) {
                throw new CalendarDataException(Organizer, "Ungültiger Organizer");
            }

            if (Endtime <= Starttime) {
                throw new CalendarDataException(this, "Endtime vor oder gleich Starttime");
            }

            if (String.IsNullOrWhiteSpace(Summary)) {
                throw new CalendarDataException(this, "Ungültiges Summary");
            }

        }

        public override string ToString() {
            var sb = new StringBuilder();

            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("UID:" + UID);
            sb.AppendLine("DTSTAMP:" + FormatDateTime(Timestamp));
            sb.AppendLine(Organizer.ToString());
            sb.AppendLine("DTSTART:" + FormatDateTime(Starttime));
            sb.AppendLine("DTEND:" + FormatDateTime(Endtime));
            sb.AppendLine("SUMMARY:" + FormatSummary(Summary));
            sb.AppendLine("END:VEVENT");

            return sb.ToString();
        }

        private static string FormatDateTime(DateTime dateTime) {
            return dateTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);
        }

        private static string FormatSummary(string text) {
            return text.RemoveNewline().Limit(Summary_Limit);
        }
    }
}
