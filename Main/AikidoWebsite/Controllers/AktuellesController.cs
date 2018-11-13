using System;
using System.Collections.Generic;
using System.Linq;
using AikidoWebsite.Common;
using AikidoWebsite.Common.VCalendar;
using AikidoWebsite.Data;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Web.Extensions;
using AikidoWebsite.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using AikidoWebsite.Data.Extensions;

namespace AikidoWebsite.Web.Controllers
{

    public class AktuellesController : Controller {

        private IDocumentSession DocumentSession { get; }
        private IClock Clock { get; }

        public AktuellesController(IDocumentSession documentSession, IClock clock)
        {
            DocumentSession = documentSession;
            Clock = clock;
        }

        [HttpGet]
        public ActionResult Index() {
            return View();
        }

        [HttpGet]
        public JsonResult GetMitteilungen(int start = 0, int perPage = 5) {
            var model = CreateListMittelungenModel(start, perPage);

            return Json(model);
        }

        [HttpGet]
        public ActionResult Mitteilung(string id) {
            var mitteilung = DocumentSession
                .Include<Mitteilung>(m => m.AutorId)
                .Load<Mitteilung>(DocumentSession.GetRavenName<Mitteilung>(id));
            var benutzer = DocumentSession.Load<Benutzer>(mitteilung.AutorId);

            var model = new ViewMitteilungModel {
                Mitteilung = MitteilungModel.Build(mitteilung, benutzer),
                Dateien = CreateDateiModels(mitteilung.DateiIds),
                Termine = CreateTerminModels(mitteilung.TerminIds)
            };

            return View(model);
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteMitteilung(string id)
        {
            // TODO: Bessere Lösung finden
            id = System.Net.WebUtility.UrlDecode(id);
            var mitteilung = DocumentSession.Load<Mitteilung>(id);
            if (mitteilung != null)
            {
                DocumentSession.Delete(mitteilung);
                DocumentSession.SaveChanges();
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        [HttpGet]
        public JsonResult Hinweis() {
            var hinweis = DocumentSession.Load<Hinweis>(DocumentSession.GetRavenName<Hinweis>("default"));

            var model = new HinweisModel();
            if (hinweis != null && hinweis.Enddatum > Clock.Now.Date)
            {
                model.HasHinweis = true;
                model.Html = hinweis.Html;
                model.DateModified = DocumentSession.Advanced.GetLastModifiedFor(hinweis);
            }

            return Json(model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult EditMitteilung() {
            return View("EditMitteilung");
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public JsonResult LoadMitteilungEditModel(string id = null)
        {
            EditMitteilungModel model;

            if (id == null)
            {
                model = new EditMitteilungModel();
            }
            else
            {
                var mitteilung = DocumentSession
                    .Include<Mitteilung>(m => m.TerminIds)
                    .Include<Datei>(m => m.DateiIds)
                    .Load(DocumentSession.GetRavenName<Mitteilung>(id));
                var termine = DocumentSession.Load<Termin>(mitteilung.TerminIds).Values;
                var dateien = CreateDateiModels(mitteilung.DateiIds);
                model = new EditMitteilungModel { Mitteilung = MitteilungModel.Build(mitteilung), Termine = termine, Dateien = dateien };
            }

            return Json(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public string ParseCreole([FromBody] CreoleModel model)
        {
            return model?.Text?.CreoleToHtml();
        }

        //[Authorize(Roles = "admin")]
        //[HttpGet]
        //public ActionResult EditNews(string id) {
        //    var mitteilung = DocumentSession.Include<Mitteilung>(m => m.TerminIds).Load(DocumentSession.GetRavenName<Mitteilung>(id));
        //    var termine = DocumentSession.Load<Termin>(mitteilung.TerminIds).Values;
        //    var model = new EditMitteilungModel { Mitteilung = MitteilungModel.Build(mitteilung), Termine = termine };

        //    // Dateien
        //    model.Dateien = CreateDateiModels(mitteilung.DateiIds);

        //    return View(model);
        //}

        [Authorize(Roles = "admin")]
        [HttpPost]
        public JsonResult EditNews(EditMitteilungModel model) {
            //var benutzer = DocumentSession.Query<Benutzer>().First(b => b.EMail.Equals(User.Identity.Name));

            //PersistTermine(model.Termine, benutzer);

            //// TODO, Validate ...
            //Mitteilung mitteilung = model.Mitteilung;
            //if (mitteilung.IsNew()) {
            //    mitteilung.AutorId = benutzer.Id;
            //    mitteilung.ErstelltAm = Clock.Now;
            //    mitteilung.TerminIds = model.Termine.Select(t => t.Id).ToSet();

            //    DocumentSession.Store(mitteilung);
            //    DocumentSession.SaveChanges();

            //    // Auf Index warten
            //    DocumentSession.Query<Mitteilung>()
            //        .Customize(c => c.WaitForNonStaleResults())
            //        .Take(0)
            //        .ToArray();

            //    return Json(mitteilung.Id);
            //} else {
            //    mitteilung = DocumentSession.Load<Mitteilung>(model.Mitteilung.Id);
            //    mitteilung.AutorId = benutzer.Id;
            //    mitteilung.Titel = model.Mitteilung.Titel;
            //    mitteilung.Text = model.Mitteilung.Text;
            //    mitteilung.TerminIds = model.Termine.Select(t => t.Id).ToSet();

            //    DocumentSession.Advanced.WaitForIndexesAfterSaveChanges();
            //    DocumentSession.SaveChanges();

            //    return Json(mitteilung.Id);
            //}
            return null;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public JsonResult UploadFile([FromForm]IFormFile file, [FromForm]string bezeichnung) {
            var key = Guid.NewGuid().ToString();

            var datei = new Datei {
                Id = DocumentSession.GetRavenName<Datei>(key),
                Name = file.FileName,
                Beschreibung = bezeichnung,
                MimeType = file.ContentType,
                Bytes = file.Length,
                AttachmentId = key
            };
            DocumentSession.Store(datei);

            using (var inputStream = file.OpenReadStream())
            {
                DocumentSession.Advanced.Attachments.Store(datei, "datei", inputStream, file.ContentType);
                DocumentSession.SaveChanges();
            }

            return Json(datei.Id);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult DeleteFile(string mitteilungsId, string fileId) {
            //var dbCommands = DocumentSession.Advanced.DocumentStore.DatabaseCommands;

            var mitteilung = DocumentSession.Load<Mitteilung>(RavenDbHelper.GetPublicName(mitteilungsId));

            if (mitteilung != null) {
                mitteilung.DateiIds.Remove(fileId);
                DocumentSession.SaveChanges();
            }

            //dbCommands.DeleteAttachment(fileId, null);

            return RedirectToAction("EditNews", new { id = mitteilungsId });
        }

        private void PersistTermine(IEnumerable<Termin> termine, Benutzer benutzer) {
            foreach (var termin in termine) {
                // TODO, Validate ...
                if (termin.IsNew()) {
                    termin.ErstellungsDatum = Clock.Now;
                    termin.AutorId = benutzer.Id;
                    termin.AutorName = benutzer.Name;

                    DocumentSession.Store(termin);
                } else {
                    var existingTermin = DocumentSession.Load<Termin>(termin.Id);
                    existingTermin.Titel = termin.Titel.Trim().RemoveNewlines();
                    existingTermin.Text = termin.Text;
                    existingTermin.StartDatum = termin.StartDatum;
                    existingTermin.EndDatum = termin.EndDatum;
                    existingTermin.Ort = termin.Ort;
                    existingTermin.Sequnce += 1;
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult RemoveNews(string id) {
            var mitteilung = DocumentSession.Load<Mitteilung>(RavenDbHelper.GetPublicName(id));

            DocumentSession.Delete(mitteilung);
            DocumentSession.SaveChanges();
            return Redirect("/Aktuelles");
        }

        [HttpGet]
        public ActionResult Termine() {
            var model = new ListTermineModel();

            return View();
        }

        [HttpGet]
        public ActionResult GetTermine()
        {
            int numberToShow = 30;
            var termine = DocumentSession.Query<Termin>()
                .Where(t => (t.EndDatum != null && t.EndDatum > Clock.Now.AddDays(-1)) || t.StartDatum > Clock.Now.AddDays(-1))
                .OrderBy(t => t.StartDatum)
                .Take(numberToShow + 1)
                .ToList();

            var model = new ListTermineModel
            {
                Termine = termine.Take(numberToShow).GroupBy(t => new DateTime(t.StartDatum.Year, t.StartDatum.Month, 1))
                    .ToDictionary(g => g.Key, g => g.ToList()),
                HasMore = termine.Count > numberToShow
            };

            return Json(model);
        }

        [ResponseCache(NoStore = true)]
        [HttpGet]
        public ActionResult RSS(int items = 10) {
            items = (items > 0 && items <= 50) ? items : 10; // Min 1, Max 50
            var rss = new RssResult("Aikido Sursee", HttpContext.GetBaseUrl(), "Aikido Sursee");

            var mitteilungen = DocumentSession.Query<Mitteilung>()
                .Include(m => m.AutorId)
                .OrderByDescending(p => p.ErstelltAm)
                .Take(items);

            foreach (var news in mitteilungen) {
                var autor = DocumentSession.Load<Benutzer>(news.AutorId);

                var url = $"{HttpContext.GetBaseUrl()}Aktuelles/Mitteilung/{RavenDbHelper.GetPublicName(news.Id)}";
                rss.AddItem(news.Titel, news.Text, url, CreatEmailWithName(autor.Name, "info@aikido-sursee.ch"), news.Id, news.ErstelltAm);
            }

            return rss;
        }

        [ResponseCache(NoStore = true)]
        [HttpGet]
        public ActionResult Ical() {
            var calendar = new Calendar();

            var startDate = Clock.Now.AddDays(-90).Date;
            var termine = DocumentSession.Query<Termin>().Where(p => p.StartDatum >= startDate);

            foreach (var termin in termine) {
                calendar.Events.Add(CreateEvent(termin));
            }

            return new ICalResult(calendar);
        }

        private IEnumerable<DateiModel> CreateDateiModels(IEnumerable<string> dateiKeys) {
            return DocumentSession.Load<Datei>(dateiKeys)
                .Where(d => d.Value != null)
                .Select(d => DateiModel.Build(d.Value));
        }

        private IEnumerable<TerminModel> CreateTerminModels(ISet<string> terminIds)
        {
            return from termin in DocumentSession.Load<Termin>(terminIds)
                   where termin.Value != null
                   select new TerminModel
                   {
                       Text = termin.Value.Text,
                       StartDatum = termin.Value.StartDatum,
                       EndDatum = termin.Value.EndDatum
                   };
        }

        private string CreatEmailWithName(string name, string email) {
            return String.Format("{0} ({1})", email, name);
        }

        private ListMitteilungenModel CreateListMittelungenModel(int start = 0, int perPage = 5) {
            var model = new ListMitteilungenModel();

            QueryStatistics stats;
            var mitteilungen = DocumentSession.Query<Mitteilung>()
                .Include(m => m.AutorId)
                .Statistics(out stats)
                .OrderByDescending(p => p.ErstelltAm)
                .Skip(start)
                .Take(perPage)
                .ToList();
            var benutzer = DocumentSession.Load<Benutzer>(mitteilungen.Select(m => m.AutorId).Distinct());

            model.Mitteilungen = mitteilungen.Select(m => MitteilungModel.Build(m, benutzer[m.AutorId]));

            model.MitteilungenCount = stats.TotalResults;
            model.Start = start;
            model.PerPage = perPage;
            model.IsAdmin = User.IsInRole("admin");

            return model;
        }

        private static CalendarEvent CreateEvent(Termin termin) {
            return new CalendarEvent {
                UID = termin.Id,
                Timestamp = termin.ErstellungsDatum,
                Starttime = termin.StartDatum,
                Endtime = termin.EndDatum ?? termin.StartDatum.AddHours(1),
                Organizer = new Organizer(termin.AutorName, "info@aikido-sursee.ch"),
                Summary = termin.Text ?? termin.Titel,
                Location = termin.Ort,
                URL = termin.URL,
                Sequnce = termin.Sequnce
            };
        }
    }
}
