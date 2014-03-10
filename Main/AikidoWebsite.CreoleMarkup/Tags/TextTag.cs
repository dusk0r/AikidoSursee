using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AikidoWebsite.CreoleMarkup.Tags {
    
    public class TextTag : Tag {

        public TextTag(params Tag[] tags) {
            Children = tags.ToList();
        }

        public override TagType Type { get { return CreoleMarkup.TagType.Text; } }
        public string Text { get; set; }

        public override string ToString() {
            return Text + String.Join("", Children.Select(t => t.ToString()));
        }
    }
}
