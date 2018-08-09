using AikidoWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AikidoWebsite.Web.Models {
    
    public class ListTermineModel {
        public IDictionary<DateTime, List<Termin>> Termine { get; set; }
        public bool HasMore { get; set; }
    }
}