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

namespace AikidoWebsite.Web.Controllers {

    public class AktuellesController : Controller {

        [Inject]
        public IMitteilungenService MitteilungenService { get; set; }

        [Inject]
        public IDocumentSession DocumentSession { get; set; }

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

            return View(model);
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
                var url = String.Format("http://www.aikido-sursee.ch/Aktuelles/Mitteilung/{0}", news.Id);
                rss.AddItem(news.Titel, news.Text, url, news.AutorName, news.AutorId, news.ErstelltAm);
            }

            return rss;
        }

        public ActionResult Mitteilung(string id, string value) {
            var docId = String.Format("{0}/{1}", id, value);
            var mitteilung = DocumentSession.Load<Mitteilung>(docId);

            return View(mitteilung);
        }

        public ActionResult Ical(string id = "alle") {
            var calendar = new Calendar();

            var termine = DocumentSession.Query<Termin>().Where(p => p.StartDatum.Day >= Clock.Now.Day);

            if (id.Equals(Publikum.Mitglieder.ToString(), StringComparison.OrdinalIgnoreCase)) {
                termine = termine.Where(m => m.Publikum == Publikum.Mitglieder);
            }

            foreach (var termin in termine.Take(10)) {
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
                Summary = termin.Text
            };
        }

    }
}
