using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace AikidoWebsite.Web.Models {
    
    public class KontaktModel {
        [Required(ErrorMessage = "Bitte geben Sie Ihren Namen an")]
        [StringLength(4, ErrorMessage = "Bitte geben Sie Ihren Namen an")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie Ihre E-Mail Adresse an")]
        [StringLength(6, ErrorMessage = "Bitte geben Sie Ihre E-Mail Adresse an")]
        public string EMail { get; set; }

        [Required(ErrorMessage = "Bitte schreiben Sie was Sie uns mitteilen wollen.")]
        public string Bemerkung { get; set; }

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