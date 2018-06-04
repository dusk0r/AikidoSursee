using System;

namespace AikidoWebsite.Common.VCalendar
{

    public class Organizer
    {
        public string Name { get; set; }
        public string EMail { get; set; }
        public bool IsValid
        {
            get
            {
                return !String.IsNullOrEmpty(Name) && !String.IsNullOrEmpty(EMail);
            }
        }

        public Organizer(string name, string email)
        {
            this.Name = Check.StringHasValue(name, "kein Name");
            this.EMail = Check.StringHasValue(email, "keine EMail Adresse");
        }

        public override string ToString()
        {
            return String.Format("ORGANIZER;CN={0}:MAILTO:{1}", Name, EMail);
        }
    }
}
