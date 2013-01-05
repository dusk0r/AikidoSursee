using AikidoWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AikidoWebsite.Web.Models {

    public class ListMitteilungenModel {
        public int MitteilungenCount { get; set; }
        public IEnumerable<Mitteilung> Mitteilungen { get; set; }
        public int Start { get; set; }

    }
}