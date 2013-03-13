using AikidoWebsite.Common;
using AikidoWebsite.Data.ValueObjects;
using Newtonsoft.Json;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.Entities {
    
    public class Termin : IEntity {

        public string Id { get; set; }
        //public string MitteilungId { get; set; }
        public string Titel { get; set; }
        public DateTime StartDatum { get; set; }
        public DateTime? EndDatum { get; set; }
        public string Ort { get; set; }
        public Publikum Publikum { get; set; }
        public string Text { get; set; }
        public string URL { get; set; }
        public string AutorId { get; set; }
        public string AutorName { get; set; }
        public DateTime ErstellungsDatum { get; set; }

        [JsonIgnore]
        public string PublikumString { get { return Publikum.ToString(); } }

        //############################################################################
        #region Object Overrides

        public override string ToString() {
            return CreateString.From(this).UseProperties().ToString();
        }

        public override bool Equals(object obj) {
            if (!(obj is Termin)) {
                return false;
            }

            var that = (Termin)obj;

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
