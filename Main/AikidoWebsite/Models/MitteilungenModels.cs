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
        public int PerPage { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class EditMitteilungModel {
        public bool WithTermin { get; set; }
        public Mitteilung Mitteilung { get; set; }
        public Termin Termin { get; set; }

        public EditMitteilungModel() {
            this.Mitteilung = new Mitteilung();
            this.Termin = new Termin();
        }
    }
}