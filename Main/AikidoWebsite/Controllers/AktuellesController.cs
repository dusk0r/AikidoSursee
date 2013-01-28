using AikidoWebsite.Common;
using AikidoWebsite.Common.VCalendar;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.Repositories;
using AikidoWebsite.Data.ValueObjects;
using AikidoWebsite.Service.Services;
using AikidoWebsite.Web.Extensions;
using AikidoWebsite.Web.Models;
using Raven.Client;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AikidoWebsite.Data;
using AikidoWebsite.Service.Validator;

namespace AikidoWebsite.Web.Controllers {

    public class AktuellesController : Controller {

        [Inject]
        public IMitteilungenService MitteilungenService { get; set; }

        [Inject]
        public IDocumentSession DocumentSession { get; set; }

        [Inject]
        public IValidatorService ValidatorService { get; set; }

        [Inject]
        public IClock Clock { get; set; }

        public ActionResult Index(int start = 0, int count = 30) {
            var model = new ListMitteilungenModel();
            //RavenQueryStatistics statistics = null;

            model.Mitteilungen = DocumentSession.Query<Mitteilung>()
                //.Statistics(out statistics)
                .OrderByDescending(p => p.ErstelltAm)
                .Skip(start)
                .Take(count);
            model.MitteilungenCount = DocumentSession.Query<Mitteilung>().Count();
            model.Start = start;
            model.IsAdmin = User.IsInGroup(Gruppe.Admin);

            return View(model);
        }

        [RequireGruppe(Gruppe.Admin)]
        public ActionResult AddNews() {
            return View("EditNews", new EditMitteilungModel());
        }

        [HttpGet]
        [RequireGruppe(Gruppe.Admin)]
        public ActionResult EditNews(string id) {
            var mitteilung = DocumentSession.Load<Mitteilung>(id.Replace('_', '/'));
            var model = new EditMitteilungModel { Mitteilung = mitteilung };

            return View(model);
        }

        [HttpPost]
        [RequireGruppe(Gruppe.Admin)]
        public JsonResult EditNews(EditMitteilungModel model) {
            var benutzer = DocumentSession.Query<Benutzer>().First(b => b.EMail.Equals(User.Identity.Name));

            Mitteilung mitteilung = model.Mitteilung;
            if (mitteilung.IsNew()) {
                mitteilung.AutorId = benutzer.Id;
                mitteilung.AutorName = benutzer.Name;
                mitteilung.ErstelltAm = Clock.Now;

                ValidatorService.Validate(mitteilung);
                DocumentSession.Store(mitteilung);
                DocumentSession.SaveChanges();
                return new JsonSaveSuccess(mitteilung.Id, "Mitteilung erstellt");
            } else {
                mitteilung = DocumentSession.Load<Mitteilung>(model.Mitteilung.Id);
                mitteilung.AutorId = benutzer.Id;
                mitteilung.AutorName = benutzer.Name;
                mitteilung.Titel = model.Mitteilung.Titel;
                mitteilung.Text = model.Mitteilung.Text;
                mitteilung.Publikum = model.Mitteilung.Publikum;

                ValidatorService.Validate(mitteilung);
                DocumentSession.SaveChanges();
                return new JsonSaveSuccess(mitteilung.Id, "Mitteilung geändert");
            }
        }

        [HttpGet]
        [RequireGruppe(Gruppe.Admin)]
        public ActionResult RemoveNews(string id) {
            var mitteilung = DocumentSession.Load<Mitteilung>(id.Replace('_', '/'));

            DocumentSession.Delete(mitteilung);
            DocumentSession.SaveChanges();
            return View("Index");
        }

        public ActionResult Termine() {
            var model = new ListTermineModel();

            model.Termine = DocumentSession.Query<Termin>()
                .Where(t => t.StartDatum > Clock.Now.AddDays(-1))
                .OrderBy(t => t.StartDatum)
                .Take(30);

            return View(model);
        }

        public ActionResult RSS(string id = "alle") {
            var rss = new RssResult("Aikido Sursee", "http://www.aikido-sursee.ch/", "Aikido Sursee");

            var mitteilungen = DocumentSession.Query<Mitteilung>().OrderByDescending(p => p.ErstelltAm);

            if (id.Equals(Publikum.Mitglieder.ToString(), StringComparison.OrdinalIgnoreCase)) {
                mitteilungen = mitteilungen.Where(m => m.Publikum == Publikum.Mitglieder);
            }

            foreach (var news in mitteilungen.Take(10)) {
                var url = String.Format("http://aikido.amigo-online.ch/Aktuelles/Mitteilung/{0}", news.Id.Replace('/', '_'));
                rss.AddItem(news.Titel, news.Text, url, news.AutorName, news.AutorId, news.ErstelltAm);
            }

            return rss;
        }

        public ActionResult Mitteilung(string id) {
            var mitteilung = DocumentSession.Load<Mitteilung>(id.Replace('_','/'));

            return View(mitteilung);
        }

        public ActionResult Ical(string id = "alle") {
            var calendar = new Calendar();

            var startDate = Clock.Now.AddDays(-90).Day;
            var termine = DocumentSession.Query<Termin>().Where(p => p.StartDatum.Day >= startDate);

            if (id.Equals(Publikum.Mitglieder.ToString(), StringComparison.OrdinalIgnoreCase)) {
                termine = termine.Where(m => m.Publikum == Publikum.Mitglieder);
            }

            foreach (var termin in termine) {
                calendar.Events.Add(CreateEvent(termin));
            }

            return new ICalResult(calendar);
        }

        private static CalendarEvent CreateEvent(Termin termin) {
            return new CalendarEvent {
                UID = termin.Id,
                Timestamp = termin.ErstellungsDatum,
                Starttime = termin.StartDatum,
                Endtime = termin.EndDatum ?? termin.StartDatum.AddHours(1),
                Organizer = new Organizer(termin.AutorName, "noreplay@aikido-sursee.ch"),
                Summary = termin.Text,
                Location = termin.Ort,
                URL = termin.URL
            };
        }

    }
}
