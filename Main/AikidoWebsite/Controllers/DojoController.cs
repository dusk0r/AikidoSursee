using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Web.Models;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AikidoWebsite.Web.Controllers {
    public class DojoController : Controller {

        [Inject]
        public IDocumentSession DocumentSession { get; set; }

        public ActionResult Trainingszeiten() {
            var plan = DocumentSession.Query<Stundenplan>().FirstOrDefault() ?? new Stundenplan();
            var model = new StundenplanModel {
                Editable = false,
                Plan = plan
            };

            model.Plan.Montag.Morgen.Titel = "Test";
            model.Plan.Montag.Morgen.Bezeichnung = "abc xyz";
            model.Plan.Montag.Morgen.Startzeit = DateTime.Now;
            model.Plan.Montag.Morgen.Endzeit = DateTime.Now.AddHours(1);

            return View(model);
        }

        public ActionResult Bilder() {
            var imageList = Directory.GetFiles(Server.MapPath("~/Content/images/dojo"))
                .Select(s => Path.GetFileName(s))
                .Where(s => s.EndsWith(".jpg") && !s.Contains("p.jpg"));

            return View(imageList);
        }

    }
}
