using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AikidoWebsite.CreoleMarkup.Tags {
    
    public abstract class Tag {
        public abstract TagType Type { get; }
        public Tag Parent { get; set; }
        public IList<Tag> Children { get; set; }
        //public abstract string StartTag { get; }
        //public abstract string EndTag { get; }

        //IEnumerable<Tag> ValidSuccsessors {  get; }

        public Tag() {
            this.Children = new List<Tag>();
        }
    }
}

