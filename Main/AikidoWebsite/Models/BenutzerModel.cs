using AikidoWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AikidoWebsite.Web.Models {
    public class BenutzerModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public IList<String> Gruppen { get; set; }
        public bool IstAktiv { get; set; }

        public BenutzerModel() {
            this.Gruppen = new List<string>();
        }

        public static BenutzerModel CreateBenutzer(Benutzer benutzer) {
            return new BenutzerModel {
                Id = benutzer.Id,
                Name = benutzer.Name,
                EMail = benutzer.EMail,
                IstAktiv = benutzer.IstAktiv,
                Gruppen = benutzer.Gruppen.Select(x => x.ToString()).ToList()
            };
        }
    }
}