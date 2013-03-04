using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.Entities {
    
    public class GlossarEintrag : IEntity {
        public string Id { get; set; }
        public string Begriff { get; set; }
        public string Text { get; set; }
    }
}
