using System;
using System.Collections.Generic;
using AikidoWebsite.Common;
using AikidoWebsite.Data.Extensions;
using Newtonsoft.Json;

namespace AikidoWebsite.Data.Entities
{

    public class Mitteilung : IEntity {
        public string Id { get; set; }
        public string Titel { get; set; }
        public DateTime ErstelltAm { get; set; }
        public string AutorId { get; set; }
        public string Text { get; set; }
        public ISet<string> TerminIds { get; set; }
        public ISet<string> DateiIds { get; set; }

        [JsonIgnore]
        public string Html { get { return Text.CreoleToHtml(); } }


        public Mitteilung() {
            this.TerminIds = new HashSet<string>();
            this.DateiIds = new HashSet<string>();
        }

        //############################################################################
        #region Object Overrides

        public override string ToString() {
            return CreateString.From(this).UseProperties().ToString();
        }

        public override bool Equals(object obj) {
            if (Object.ReferenceEquals(this, obj)) {
                return true;
            }

            var other = obj as Mitteilung;

            if (other == null) {
                return false;
            }

            return (Id == null) ? Id.Equals(other.Id) : false;
        }

        public override int GetHashCode() {
            return (Id == null) ? base.GetHashCode() : Id.GetHashCode();
        }

        #endregion
        //############################################################################

    }
}
