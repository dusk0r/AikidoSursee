using AikidoWebsite.Common;
using Raven.Imports.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.Entities {
    
    public class Datei : IEntity {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Beschreibung { get; set; }
        public string AttachmentId { get; set; }
        public string MimeType { get; set; }
        public int Bytes { get; set; }

        [JsonIgnore]
        public bool IsImage { get { return MimeType.StartsWith("image", StringComparison.OrdinalIgnoreCase); } }

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
