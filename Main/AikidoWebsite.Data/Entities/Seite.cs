using System;
using AikidoWebsite.Data.Extensions;
using Newtonsoft.Json;

namespace AikidoWebsite.Data.Entities
{
    public class Seite : IEntity {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime ErstellungsDatum { get; set; }
        public string Autor { get; set; }
        public int Revision { get; set; }
        public string WikiCreole { get; set; }

        [JsonIgnore]
        public string Html { get { return WikiCreole.CreoleToHtml(); } }

        public Seite Copy() {
            return new Seite {
                Id = Id,
                Name = Name,
                ErstellungsDatum = ErstellungsDatum,
                Autor = Autor,
                Revision = Revision,
                WikiCreole = WikiCreole
            };
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

            var other = obj as Seite;

            if (other == null) {
                return false;
            }

            return (Id == null) ? false : Id.Equals(other.Id) && Revision.Equals(other.Revision);
        }

        public override int GetHashCode() {
            return (Id == null) ? base.GetHashCode() : HashCode.FromObjects(Id, Revision);
        }

        #endregion
        //############################################################################
    }
}
