using AikidoWebsite.Common;
using AikidoWebsite.Data.ValueObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wiki;

namespace AikidoWebsite.Data.Entities {

    public class Mitteilung : IEntity {
        private static readonly CreoleParser CreoleParser = new CreoleParser();

        public string Id { get; set; }
        public string Titel { get; set; }
        public DateTime ErstelltAm { get; set; }
        public string AutorId { get; set; }
        public string AutorName { get; set; }
        public string AutorEmail { get; set; }
        public string Text { get; set; }
        // Todo, custom converter nötig?
        public Publikum Publikum { get; set; }
        public ISet<string> TerminIds { get; set; }
        public ISet<string> DateiIds { get; set; }

        [JsonIgnore]
        public string Html { get { return CreoleParser.ToHTML(Text ?? ""); } }

        [JsonIgnore]
        public string PublikumString { get { return Publikum.ToString(); } }

        public Mitteilung() {
            this.AutorEmail = "nobody@amigo-online.ch";
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
