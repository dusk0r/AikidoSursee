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

            sb.Append("BEGIN:VEVENT\r\n");
            sb.Append("UID:" + UID + "\r\n");
            sb.Append("SEQUENCE:" + Sequnce + "\r\n");
            sb.Append("DTSTAMP:" + FormatDateTime(Timestamp) + "\r\n");
            sb.Append(Organizer.ToString() + "\r\n");
            sb.Append("DTSTART:" + FormatDateTime(Starttime) + "\r\n");
            sb.Append("DTEND:" + FormatDateTime(Endtime) + "\r\n");
            sb.Append("SUMMARY:" + FormatText(Summary) + "\r\n");
            if (!String.IsNullOrWhiteSpace(URL))
            {
                sb.Append("URL:" + URL + "\r\n");
            }
            if (!String.IsNullOrWhiteSpace(Location))
            {
                sb.Append("LOCATION:" + FormatText(Location) + "\r\n");
            }
            sb.Append("END:VEVENT" + "\r\n");

            return sb.ToString();
        }

        private static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);
        }

        private static string FormatText(string text)
        {
            return text.RemoveNewlines().Limit(Summary_Limit);
        }
    }
}
