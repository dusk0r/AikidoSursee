using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AikidoWebsite.Web.Models {

    public class SeiteModel {
        public string Name { get; set; }
        public int Revision { get; set; }
        public string Markdown { get; set; }
    }

}
