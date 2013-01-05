using AikidoWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AikidoWebsite.Web.Models {
    
    public class ListTermineModel {
        public IEnumerable<Termin> Termine { get; set; }
    }
}