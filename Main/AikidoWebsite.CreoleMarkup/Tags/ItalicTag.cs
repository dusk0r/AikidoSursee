using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AikidoWebsite.CreoleMarkup.Tags {
    
    public class ItalicTag : Tag {

        public ItalicTag(params Tag[] tags) {
            Children = tags.ToList();
        }

        public override TagType Type { get { return CreoleMarkup.TagType.Italics; } }
        //public override string StartTag { get { return "//"; } }
        //public override string EndTag { get { return "//"; } }

        //public string Process(string source) {
        //    return "<i>" + source.Substring(2, source.Length - 4) + "</i>";
        //}

        //public IEnumerable<Tag> ValidSuccsessors {
        //    get { return new Tag[] { Tag.Bold, Tag.Linebreak, Tag.Link, Tag.No_markup, Tag.Raw }; }
        //}

        public override string ToString() {
            return "<i>" + String.Join("", Children.Select(t => t.ToString())) + "</i>";
        }
    }
}
