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
using Raven.Client.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AikidoWebsite.Data;
using AikidoWebsite.Service.Validator;
using Raven.Json.Linq;

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

        [HttpGet]
        public ActionResult Index() {
            var model = CreateListMittelungenModel();

            return View(model);
        }

        [HttpPost]
        public JsonResult GetMitteilungen(int start = 0, int perPage = 5) {
            var model = CreateListMittelungenModel(start, perPage);

            return Json(model);
        }

        [HttpGet]
        public ActionResult Mitteilung(string id) {
            var mitteilung = DocumentSession.Load<Mitteilung>(RavenDbHelper.DecodeDocumentId(id));
            var model = new ViewMitteilungModel { Mitteilung = mitteilung, Dateien = CreateDateiModels(mitteilung.DateiIds) };

            return View(model);
        }

        [RequireGruppe(Gruppe.Admin)]
        public ActionResult AddNews() {
            return View("EditNews", new EditMitteilungModel());
        }

        [HttpGet]
        [RequireGruppe(Gruppe.Admin)]
        public ActionResult EditNews(string id) {
            var mitteilung = DocumentSession.Include<Mitteilung>(m => m.TerminIds).Load(RavenDbHelper.DecodeDocumentId(id));
            var termine = DocumentSession.Load<Termin>(mitteilung.TerminIds);
            var model = new EditMitteilungModel { Mitteilung = mitteilung, Termine = termine };

            // Dateien
            model.Dateien = CreateDateiModels(mitteilung.DateiIds);

            return View(model);
        }

        [HttpPost]
        [RequireGruppe(Gruppe.Admin)]
        public JsonResult EditNews(EditMitteilungModel model) {
            var benutzer = DocumentSession.Query<Benutzer>().First(b => b.EMail.Equals(User.Identity.Name));

            PersistTermine(model.Termine, model.Mitteilung.Publikum, benutzer);

            Mitteilung mitteilung = model.Mitteilung;
            if (mitteilung.IsNew()) {
                mitteilung.AutorId = benutzer.Id;
                mitteilung.AutorName = benutzer.Name;
                mitteilung.AutorEmail = benutzer.EMail;
                mitteilung.ErstelltAm = Clock.Now;
                mitteilung.TerminIds = model.Termine.Select(t => t.Id).ToSet();

                ValidatorService.Validate(mitteilung);
                DocumentSession.Store(mitteilung);

                DocumentSession.SaveChanges();
                return new JsonSaveSuccess(mitteilung.Id, "Mitteilung erstellt");
            } else {
                mitteilung = DocumentSession.Load<Mitteilung>(model.Mitteilung.Id);
                mitteilung.AutorId = benutzer.Id;
                mitteilung.AutorName = benutzer.Name;
                mitteilung.AutorEmail = benutzer.EMail;
                mitteilung.Titel = model.Mitteilung.Titel;
                mitteilung.Text = model.Mitteilung.Text;
                mitteilung.Publikum = model.Mitteilung.Publikum;
                mitteilung.TerminIds = model.Termine.Select(t => t.Id).ToSet();

                ValidatorService.Validate(mitteilung);
                
                DocumentSession.SaveChanges();
                return new JsonSaveSuccess(mitteilung.Id, "Mitteilung geändert");
            }
        }

        [HttpPost]
        [RequireGruppe(Gruppe.Admin)]
        public ActionResult UploadFile(HttpPostedFileBase file, string bezeichnung, string mitteilungsId) {
            var dbCommands = DocumentSession.Advanced.DatabaseCommands;
            
            var mitteilung = DocumentSession.Load<Mitteilung>(mitteilungsId);
            var key = Guid.NewGuid().ToString();
            var metadata = new RavenJObject();

            metadata["Bezeichnung"] = bezeichnung;
            metadata["DateiName"] = file.FileName;
            metadata["ContentType"] = file.ContentType;
            dbCommands.PutAttachment(key, null, file.InputStream, metadata);

            mitteilung.DateiIds.Add(key);
            DocumentSession.SaveChanges();

            return RedirectToAction("EditNews", new { id = RavenDbHelper.EncodeDocumentId(mitteilungsId) });
        }

        [HttpGet]
        [RequireGruppe(Gruppe.Admin)]
        public ActionResult DeleteFile(string mitteilungsId, string fileId) {
            var dbCommands = DocumentSession.Advanced.DatabaseCommands;

            var mitteilung = DocumentSession.Load<Mitteilung>(RavenDbHelper.DecodeDocumentId(mitteilungsId));

            if (mitteilung != null) {
                mitteilung.DateiIds.Remove(fileId);
                DocumentSession.SaveChanges();
            }

            dbCommands.DeleteAttachment(fileId, null);

            return RedirectToAction("EditNews", new { id = mitteilungsId });
        }

        private void PersistTermine(IEnumerable<Termin> termine, Publikum publikum, Benutzer benutzer) {
            foreach (var termin in termine) {
                termin.Publikum = publikum;
                if (termin.IsNew()) {
                    //termin.MitteilungId = mitteilung.Id;
                    termin.ErstellungsDatum = Clock.Now;
                    termin.AutorId = benutzer.Id;
                    termin.AutorName = benutzer.Name;
                    //termin.URL = ...

                    ValidatorService.Validate(termin);
                    DocumentSession.Store(termin);
                } else {
                    var existingTermin = DocumentSession.Load<Termin>(termin.Id);
                    existingTermin.Titel = termin.Titel;
                    existingTermin.Text = termin.Text;
                    existingTermin.StartDatum = termin.StartDatum;
                    existingTermin.EndDatum = termin.EndDatum;
                    existingTermin.Ort = termin.Ort;

                    ValidatorService.Validate(termin);
                }

            }
        }

        [HttpGet]
        [RequireGruppe(Gruppe.Admin)]
        public ActionResult RemoveNews(string id) {
            var mitteilung = DocumentSession.Load<Mitteilung>(RavenDbHelper.DecodeDocumentId(id));

            DocumentSession.Delete(mitteilung);
            DocumentSession.SaveChanges();
            return Redirect("/Aktuelles");
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

            //if (id.Equals(Publikum.Extern.ToString(), StringComparison.OrdinalIgnoreCase)) {
            //    mitteilungen = mitteilungen.Where(m => m.Publikum == Publikum.Extern);
            //}
            //if (id.Equals(Publikum.Sursee.ToString(), StringComparison.OrdinalIgnoreCase)) {
            //    mitteilungen = mitteilungen.Where(m => m.Publikum == Publikum.Sursee);
            //}

            foreach (var news in mitteilungen.Take(10)) {
                var url = String.Format("http://aikido.amigo-online.ch/Aktuelles/Mitteilung/{0}", RavenDbHelper.EncodeDocumentId(news.Id));
                rss.AddItem(news.Titel, news.Text, url, CreatEmailWithName(news.AutorName, news.AutorEmail), news.Id, news.ErstelltAm);
            }

            return rss;
        }

        public ActionResult Ical(string id = "alle") {
            var calendar = new Calendar();

            var startDate = Clock.Now.AddDays(-90).Date;
            var termine = DocumentSession.Query<Termin>().Where(p => p.StartDatum >= startDate);

            //if (id.Equals(Publikum.Extern.ToString(), StringComparison.OrdinalIgnoreCase)) {
            //    termine = termine.Where(m => m.Publikum == Publikum.Extern);
            //}
            //if (id.Equals(Publikum.Sursee.ToString(), StringComparison.OrdinalIgnoreCase)) {
            //    termine = termine.Where(m => m.Publikum == Publikum.Sursee);
            //}

            foreach (var termin in termine) {
                calendar.Events.Add(CreateEvent(termin));
            }

            return new ICalResult(calendar);
        }

        private IEnumerable<DateiModel> CreateDateiModels(IEnumerable<string> dateiKeys) {
            var dbCommands = DocumentSession.Advanced.DatabaseCommands;

            return dateiKeys.Select(g => {
                var attachment = dbCommands.GetAttachment(g);
                return new DateiModel {
                    Id = g,
                    Bezeichnung = attachment.Metadata["Bezeichnung"].ToString(),
                    DateiName = attachment.Metadata["DateiName"].ToString(),
                    Size = attachment.Size
                };
            }).ToList();
        }

        private string CreatEmailWithName(string name, string email) {
            return String.Format("{0} ({1})", email, name);
        }

        private ListMitteilungenModel CreateListMittelungenModel(int start = 0, int perPage = 5) {
            var model = new ListMitteilungenModel();

            RavenQueryStatistics stats;
            model.Mitteilungen = DocumentSession.Query<Mitteilung>()
            .Customize(a => a.WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(2)))
            .Statistics(out stats)
            .OrderByDescending(p => p.ErstelltAm)
            .Skip(start)
            .Take(perPage)
            .ToList();

            //model.MitteilungenCount = DocumentSession.Query<Mitteilung>().Count();
            model.MitteilungenCount = stats.TotalResults;
            model.Start = start;
            model.PerPage = perPage;
            model.IsAdmin = User.IsInGroup(Gruppe.Admin);

            return model;
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
