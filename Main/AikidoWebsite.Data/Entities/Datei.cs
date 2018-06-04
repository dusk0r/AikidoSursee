using System;
using AikidoWebsite.Common;
using Newtonsoft.Json;

namespace AikidoWebsite.Data.Entities
{

    public class Datei : IEntity {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Beschreibung { get; set; }
        public string AttachmentId { get; set; }
        public string MimeType { get; set; }
        public long Bytes { get; set; }

        [JsonIgnore]
        public bool IsImage => MimeType.StartsWith("image", StringComparison.OrdinalIgnoreCase);

        //############################################################################
        #region Object Overrides

        public override string ToString() {
            return CreateString.From(this).UseProperties().ToString();
        }

        public override bool Equals(object obj) {
            if (Object.ReferenceEquals(this, obj)) {
                return true;
            }

            var other = obj as Datei;

            return (other == null) ? false : Id.Equals(other.Id);
        }

        public override int GetHashCode() {
            return (Id == null) ? base.GetHashCode() : Id.GetHashCode();
        }

        #endregion
        //############################################################################
    }
}
