using AikidoWebsite.Common;
using AikidoWebsite.Data.ValueObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.Entities {

    public class Mitteilung : IEntity {

        public string Id { get; set; }
        public string Titel { get; set; }
        public DateTime ErstelltAm { get; set; }
        public string AutorId { get; set; }
        public string AutorName { get; set; }
        public string AutorEmail { get; set; }
        public string Text { get; set; }
        // Todo, custom converter nötig?
        public Publikum Publikum { get; set; }

        [JsonIgnore]
        public string PublikumString { get { return Publikum.ToString(); } }

        public Mitteilung() {
            AutorEmail = "nobody@amigo-online.ch";
        }

        //############################################################################
        #region Object Overrides

        public override string ToString() {
            return CreateString.From(this).UseProperties().ToString();
        }

        public override bool Equals(object obj) {
            if (!(obj is Mitteilung)) {
                return false;
            }

            var that = (Mitteilung)obj;

            if (this.Id != null) {
                return this.Id.Equals(that.Id);
            } else {
                return Object.ReferenceEquals(this, that);
            }
        }

        public override int GetHashCode() {
            return (Id == null) ? ((object)this).GetHashCode() : Id.GetHashCode();
        }

        #endregion
        //############################################################################

    }
}
