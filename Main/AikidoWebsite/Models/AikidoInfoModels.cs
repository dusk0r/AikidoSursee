using AikidoWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AikidoWebsite.Web.Models {
    
    public class FaqModel {
        public bool IsAdmin { get; set; }
        public IEnumerable<FaqEintrag> Entries { get; set; }
    }

    public class GlossarModel {
        public bool IsAdmin { get; set; }
        public IEnumerable<GlossarEintrag> Entries { get; set; }
    }

    
}