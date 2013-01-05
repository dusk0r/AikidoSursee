using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.Identities {
    
    public class Identity<T> where T : IEntity {
        public string Id { get; private set; }

        public Identity(string id) {
            this.Id = Check.StringHasValue(id, "ID fehlt");
        }

        public bool Is(T that) {
            return String.Equals(this.Id, that.Id);
        }
    }
}
