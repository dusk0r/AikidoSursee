using System;
using System.Globalization;
using System.Text;

namespace AikidoWebsite.Common.VCalendar
{

    public class CalendarEvent
    {
        private static readonly int Summary_Limit = 100;

        public string UID { get; set; }
        public int Sequnce { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime Starttime { get; set; }
        public DateTime Endtime { get; set; }
        public Organizer Organizer { get; set; }
        public string Location { get; set; }
        public string Summary { get; set; }
        public string URL { get; set; }

        public bool IsValid
        {
            get
            {
                try
                {
                    Check();
                    return true;
                }
                catch (CalendarDataException)
                {
                    return false;
                }
            }
        }

        public void Check()
        {
            if (String.IsNullOrWhiteSpace(UID))
            {
                throw new CalendarDataException(this, "Keine UID");
            }

            if (Organizer == null)
            {
                throw new CalendarDataException(this, "Kein Organizer");
            }

            if (!Organizer.IsValid)
            {
                throw new CalendarDataException(Organizer, "Ungültiger Organizer");
            }

            if (Endtime <= Starttime)
            {
                throw new CalendarDataException(this, "Endtime vor oder gleich Starttime");
            }

            if (String.IsNullOrWhiteSpace(Summary))
            {
                throw new CalendarDataException(this, "Ungültiges Summary");
            }

        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("UID:" + UID);
            sb.AppendLine("SEQUENCE:" + Sequnce);
            sb.AppendLine("DTSTAMP:" + FormatDateTime(Timestamp));
            sb.AppendLine(Organizer.ToString());
            sb.AppendLine("DTSTART:" + FormatDateTime(Starttime));
            sb.AppendLine("DTEND:" + FormatDateTime(Endtime));
            sb.AppendLine("SUMMARY:" + FormatText(Summary));
            if (!String.IsNullOrWhiteSpace(URL))
            {
                sb.AppendLine("URL:" + URL);
            }
            if (!String.IsNullOrWhiteSpace(Location))
            {
                sb.AppendLine("LOCATION:" + FormatText(Location));
            }
            sb.AppendLine("END:VEVENT");

            return sb.ToString();
        }

        private static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);
        }

        private static string FormatText(string text)
        {
            return text.RemoveNewline().Limit(Summary_Limit);
        }
    }
}
