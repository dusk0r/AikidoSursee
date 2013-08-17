using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AikidoWebsite.Web.Models {
    
    public class KontaktModel {
        public string Name { get; set; }
        public string EMail { get; set; }
        public string Bemerkung { get; set; }
        public string Code { get; set; }

        public string FormatText() {
            var sb = new StringBuilder();

            sb.Append("Name: ");
            sb.AppendLine(Name);
            sb.Append("EMail: ");
            sb.AppendLine(EMail);
            sb.Append("Datum: ");
            sb.AppendLine(DateTime.Now.ToString());
            sb.Append("Bemerkung: ");
            sb.AppendLine(Bemerkung);

            return sb.ToString();
        }
    }
}