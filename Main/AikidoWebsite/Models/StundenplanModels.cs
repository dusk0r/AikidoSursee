using AikidoWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AikidoWebsite.Web.Models {
    
    public class StundenplanModel {
        public bool Editable { get; set; }
        public Stundenplan Plan { get; set; }
    }
}