using System;
using System.Collections.Generic;
using System.Linq;
using AikidoWebsite.Common;
using AikidoWebsite.Common.VCalendar;
using AikidoWebsite.Data;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.ValueObjects;
using AikidoWebsite.Web.Extensions;
using AikidoWebsite.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

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
            var mitteilung = DocumentSession
                .Include<Mitteilung>(m => m.AutorId)
                .Load<Mitteilung>(RavenDbHelper.DecodeDocumentId(id));
            var benutzer = DocumentSession.Load<Benutzer>(mitteilung.AutorId);

            var model = new ViewMitteilungModel { Mitteilung = CreateMitteilungModel(mitteilung, benutzer), Dateien = CreateDateiModels(mitteilung.DateiIds) };

            return View(model);
        }

        [HttpGet]
        public JsonResult Hinweis() {
            var hinweis = DocumentSession.Load<Hinweis>("default");
            var metadata = DocumentSession.Advanced.GetMetadataFor(hinweis);
            var etag = metadata.GetString("@etag");

            return Json(new { Hinweis = hinweis.Html, Tag = etag.ToString() });
        }

        [Authorize(Roles = "admin")]
        public ActionResult AddNews() {
            return View("EditNews", new EditMitteilungModel());
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult EditNews(string id) {
            var mitteilung = DocumentSession.Include<Mitteilung>(m => m.TerminIds).Load(RavenDbHelper.DecodeDocumentId(id));
            var termine = DocumentSession.Load<Termin>(mitteilung.TerminIds).Values;
            var model = new EditMitteilungModel { Mitteilung = mitteilung, Termine = termine };

            // Dateien
            model.Dateien = CreateDateiModels(mitteilung.DateiIds);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public JsonResult EditNews(EditMitteilungModel model) {
            var benutzer = DocumentSession.Query<Benutzer>().First(b => b.EMail.Equals(User.Identity.Name));

            PersistTermine(model.Termine, benutzer);

            // TODO, Validate ...
            Mitteilung mitteilung = model.Mitteilung;
            if (mitteilung.IsNew()) {
                mitteilung.AutorId = benutzer.Id;
                mitteilung.ErstelltAm = Clock.Now;
                mitteilung.TerminIds = model.Termine.Select(t => t.Id).ToSet();

                DocumentSession.Store(mitteilung);
                DocumentSession.SaveChanges();

                // Auf Index warten
                DocumentSession.Query<Mitteilung>()
                    .Customize(c => c.WaitForNonStaleResults())
                    .Take(0)
                    .ToArray();

                return Json(mitteilung.Id);
            } else {
                mitteilung = DocumentSession.Load<Mitteilung>(model.Mitteilung.Id);
                mitteilung.AutorId = benutzer.Id;
                mitteilung.Titel = model.Mitteilung.Titel;
                mitteilung.Text = model.Mitteilung.Text;
                mitteilung.TerminIds = model.Termine.Select(t => t.Id).ToSet();
                
                DocumentSession.SaveChanges();

                // Auf Index warten
                DocumentSession.Query<Mitteilung>()
                    .Customize(c => c.WaitForNonStaleResults())
                    .Take(0)
                    .ToArray();

                return Json(mitteilung.Id);
            }

        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult UploadFile([FromBody]IFormFile file, [FromBody]string bezeichnung, [FromBody]string mitteilungsId) {
            //var dbCommands = DocumentSession.Advanced.DocumentStore.DatabaseCommands;
            
            var mitteilung = DocumentSession.Load<Mitteilung>(mitteilungsId);
            var key = Guid.NewGuid().ToString();

            // Entity Speichern
            var datei = new Datei {
                Name = file.FileName,
                Beschreibung = bezeichnung,
                MimeType = file.ContentType,
                Bytes = file.Length,
                AttachmentId = key
            };
            DocumentSession.Store(datei);

            var metadata = new JObject();
            metadata["Bezeichnung"] = bezeichnung;
            metadata["DateiName"] = file.FileName;
            metadata["ContentType"] = file.ContentType;
            //dbCommands.PutAttachment(key, null, file.InputStream, metadata); // TODO: Implementieren

            mitteilung.DateiIds.Add(key);
            DocumentSession.SaveChanges();

            return RedirectToAction("EditNews", new { id = RavenDbHelper.EncodeDocumentId(mitteilungsId) });
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteFile(string mitteilungsId, string fileId) {
            //var dbCommands = DocumentSession.Advanced.DocumentStore.DatabaseCommands;

            var mitteilung = DocumentSession.Load<Mitteilung>(RavenDbHelper.DecodeDocumentId(mitteilungsId));

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
                    //termin.MitteilungId = mitteilung.Id;
                    termin.ErstellungsDatum = Clock.Now;
                    termin.AutorId = benutzer.Id;
                    termin.AutorName = benutzer.Name;
                    //termin.URL = ...

                    DocumentSession.Store(termin);
                } else {
                    var existingTermin = DocumentSession.Load<Termin>(termin.Id);
                    existingTermin.Titel = termin.Titel;
                    existingTermin.Text = termin.Text;
                    existingTermin.StartDatum = termin.StartDatum;
                    existingTermin.EndDatum = termin.EndDatum;
                    existingTermin.Ort = termin.Ort;

                }

            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
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

        [ResponseCache(NoStore = true)]
        public ActionResult RSS() {
            var rss = new RssResult("Aikido Sursee", "http://www.aikido-sursee.ch/", "Aikido Sursee");

            // Todo, 10 kofigurierbar machen
            var mitteilungen = DocumentSession.Query<Mitteilung>()
                .Include(m => m.AutorId)
                .OrderByDescending(p => p.ErstelltAm)
                .Take(10);

            foreach (var news in mitteilungen) {
                var autor = DocumentSession.Load<Benutzer>(news.AutorId);

                // TODO: URL dynamisch machen
                var url = String.Format("http://aikido.amigo-online.ch/Aktuelles/Mitteilung/{0}", RavenDbHelper.EncodeDocumentId(news.Id));
                rss.AddItem(news.Titel, news.Text, url, CreatEmailWithName(autor.Name, "info@aikido-sursee.ch"), news.Id, news.ErstelltAm);
            }

            return rss;
        }

        [ResponseCache(NoStore = true)]
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
            //var dbCommands = DocumentSession.Advanced.DocumentStore.DatabaseCommands;

            return dateiKeys.Select(g => {
                //var attachment = dbCommands.GetAttachment(g); // TODO: Implementieren
                return new DateiModel {
                    Id = g,
                    //Bezeichnung = attachment.Metadata["Bezeichnung"].ToString(),
                    //DateiName = attachment.Metadata["DateiName"].ToString(),
                    //Size = attachment.Size
                };
            }).ToList();
        }

        private string CreatEmailWithName(string name, string email) {
            return String.Format("{0} ({1})", email, name);
        }

        private ListMitteilungenModel CreateListMittelungenModel(int start = 0, int perPage = 5) {
            var model = new ListMitteilungenModel();

            // Todo, Lazy
            QueryStatistics stats;
            var mitteilungen = DocumentSession.Query<Mitteilung>()
            .Include(m => m.AutorId)
            .Statistics(out stats)
            .OrderByDescending(p => p.ErstelltAm)
            .Skip(start)
            .Take(perPage)
            .ToList();
            var benutzer = DocumentSession.Load<Benutzer>(mitteilungen.Select(m => m.AutorId)).ToList();

            model.Mitteilungen = mitteilungen.Select(m => CreateMitteilungModel(m, DocumentSession.Load<Benutzer>(m.AutorId)));

            //model.MitteilungenCount = DocumentSession.Query<Mitteilung>().Count();
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

        public static MitteilungModel CreateMitteilungModel(Mitteilung mitteilung, Benutzer benutzer) {
            return new MitteilungModel {
                Id = mitteilung.Id,
                Titel = mitteilung.Titel,
                ErstelltAm = mitteilung.ErstelltAm,
                AutorId = benutzer.Id,
                AutorName = benutzer.Name,
                AutorEmail = benutzer.EMail,
                Text = mitteilung.Text,
                TerminIds = mitteilung.TerminIds,
                DateiIds = mitteilung.DateiIds,
                Html = mitteilung.Html
            };
        }

    }
}
