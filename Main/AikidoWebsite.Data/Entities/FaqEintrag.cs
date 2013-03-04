using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.Entities {
    
    public class FaqEintrag : IEntity {
        public string Id { get; set; }
        public string Frage { get; set; }
        public string Text { get; set; }

    }
}
