using System;
using AikidoWebsite.Common;

namespace AikidoWebsite.Data.Entities
{

    public class Termin : IEntity {

        public string Id { get; set; }
        //public string MitteilungId { get; set; }
        public string Titel { get; set; }
        public DateTime StartDatum { get; set; }
        public DateTime? EndDatum { get; set; }
        public string Ort { get; set; }
        public string Text { get; set; }
        public string URL { get; set; }
        public string AutorId { get; set; }
        public string AutorName { get; set; }
        public DateTime ErstellungsDatum { get; set; }
        public int Sequnce { get; set; }


        //############################################################################
        #region Object Overrides

        public override string ToString() {
            return CreateString.From(this).UseProperties().ToString();
        }

        public override bool Equals(object obj) {
            if (Object.ReferenceEquals(this, obj)) {
                return true;
            }

            var other = obj as Termin;

            return (other == null) ? false : Id.Equals(other.Id);
        }

        public override int GetHashCode() {
            return (Id == null) ? base.GetHashCode() : Id.GetHashCode();
        }

        #endregion
        //############################################################################

    }
}
