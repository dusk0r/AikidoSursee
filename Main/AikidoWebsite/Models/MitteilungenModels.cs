using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AikidoWebsite.Web.Models {

    public class ListMitteilungenModel {
        public int MitteilungenCount { get; set; }
        public IEnumerable<MitteilungModel> Mitteilungen { get; set; }
        public int Start { get; set; }
        public int PerPage { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class ViewMitteilungModel {
        public MitteilungModel Mitteilung { get; set; }
        public IEnumerable<DateiModel> Dateien { get; set; }

        public ViewMitteilungModel() {

            this.Dateien = new List<DateiModel>();
        }
    }

    public class MitteilungModel {
        public string Id { get; set; }
        public string Titel { get; set; }
        public DateTime ErstelltAm { get; set; }
        public string AutorId { get; set; }
        public string AutorName { get; set; }
        public string AutorEmail { get; set; }
        public string Text { get; set; }
        public ISet<string> TerminIds { get; set; }
        public ISet<string> DateiIds { get; set; }

        // Todo, Nicht direkt parsen? Oder Text weglassen?
        public string Html { get; set; }
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