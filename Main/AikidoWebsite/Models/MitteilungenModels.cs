﻿using AikidoWebsite.Data.Entities;
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

    public class ViewMitteilungModel {
        public Mitteilung Mitteilung { get; set; }
        public IEnumerable<DateiModel> Dateien { get; set; }

        public ViewMitteilungModel() {

            this.Dateien = new List<DateiModel>();
        }
    }

    public class EditMitteilungModel {
        public bool WithTermin { get; set; }
        public Mitteilung Mitteilung { get; set; }
        public IEnumerable<Termin> Termine { get; set; }
        public IEnumerable<DateiModel> Dateien { get; set; }

        public EditMitteilungModel() {
            this.Mitteilung = new Mitteilung();
            this.Termine = new List<Termin>();
            this.Dateien = new List<DateiModel>();
        }
    }

    public class DateiModel {
        public string DateiName { get; set; }
        public string Bezeichnung { get; set; }
        public string Id { get; set; }
        public string ContentType { get; set; }
        public int Size { get; set; }
    }
}