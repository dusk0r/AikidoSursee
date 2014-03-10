using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AikidoWebsite.CreoleMarkup.Tags {
    
    public class BoldTag : Tag {

        public BoldTag(params Tag[] tags) {
            Children = tags.ToList();
        }

        public override TagType Type { get { return CreoleMarkup.TagType.Bold; } }

        //public override string StartTag { get { return "**"; } }
        //public override string EndTag { get { return "**"; } }

        public override string ToString() {
            return "<b>" + String.Join("", Children.Select(t => t.ToString())) + "</b>";
        }
    }
}
