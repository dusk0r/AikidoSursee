using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.Entities {
    public class Article : IEntity {
        public string Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Autor { get; set; }
        public int Revision { get; set; }
        public IEnumerable<Article> OldRevisions { get; set; }
        public string Markdown { get; set; }

        [JsonIgnore]
        public string Html { get; }
    }
}
