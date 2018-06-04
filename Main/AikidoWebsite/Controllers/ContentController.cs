using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.Index;
using AikidoWebsite.Data.ValueObjects;
using AikidoWebsite.Web.Extensions;
using AikidoWebsite.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Raven.Client.Documents.Session;
using Raven.Client.Extensions;

namespace AikidoWebsite.Web.Controllers
{

    public class ContentController : Controller {

        private IDocumentSession DocumentSession { get; }
        private IClock Clock { get; }

        public ContentController(IDocumentSession documentSession, IClock clock)
        {
            DocumentSession = documentSession;
            Clock = clock;
        }

        [HttpGet]
        public ActionResult Show(string id) {
            var article = DocumentSession.Query<Seite, AktuelleSeiteIndex>()
                .FirstOrDefault(a => a.Name == id);

            if (article != null) {
                return View(article);
            } else {
                Response.StatusCode = 404;
                //Response.TrySkipIisCustomErrors = true;
                return View("ArtikelNichtGefunden", (object)id);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Edit(string id, bool saved = false) {
            var article = DocumentSession.Query<Seite, AktuelleSeiteIndex>()
                .FirstOrDefault(a => a.Name == id);

            var model = new SeiteModel {
                Name = id,
                Revision = (article != null) ? article.Revision : 0,
                Markdown = (article != null) ? article.WikiCreole : "",
                Html = (article != null) ? article.Html : "",
                Saved = saved
            };

            ViewData["Files"] = DocumentSession.Query<Datei>()
                .Customize(x => x.WaitForNonStaleResults())
                .ToList();

            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public JsonResult Edit(SeiteModel model) {
            var site = DocumentSession.Query<Seite, AktuelleSeiteIndex>()
                .FirstOrDefault(a => a.Name == model.Name);
            var benutzer = DocumentSession.Query<Benutzer>()
                .First(b => b.EMail ==User.Identity.Name);

            var oldRevisions = new HashSet<Seite>();

            if (site == null) {
                site = new Seite {
                    Revision = 0,
                    Name = model.Name
                };
            } else {
                // Revisionen aufräumen
                DocumentSession.Store(site.Copy(), site.Id + "/revision/" + site.Revision);
            }

            // Update
            site.ErstellungsDatum = Clock.Now;
            site.Autor = benutzer.Name;
            site.WikiCreole = model.Markdown;
            site.Revision += 1;

            DocumentSession.Store(site);
            DocumentSession.SaveChanges();
            DocumentSession.Query<AktuelleSeiteIndex>()
                .Customize(c => c.WaitForNonStaleResults())
                .Take(0)
                .ToArray();

            return Json(site.Id);
        }

        [HttpGet]
        public ActionResult File(string id) {
            //var dbCommands = DocumentSession.Advanced.DocumentStore.DatabaseCommands;

            // TODO: Implementieren
            //var attachment = dbCommands.GetAttachment(id);

            //if (attachment == null) {
                return StatusCode((int)System.Net.HttpStatusCode.NotFound);

            //} else {
            //    var contentType = attachment.Metadata["ContentType"].ToString();

            //    return File(attachment.Data(), contentType);
            //}
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [ResponseCache(NoStore = true)]
        public ActionResult Files() {
            // Todo: Paging
            QueryStatistics stats = null;
            var files = DocumentSession.Query<Datei>()
                .Customize(x => x.WaitForNonStaleResults())
                .Statistics(out stats)
                .ToList();

            var fileUsage = DocumentSession.Query<FileUsageCountIndex.Result, FileUsageCountIndex>()
                //.Where(x => x.AttachmentId.In(files.Select(f => f.AttachmentId))) // TODO: Implementieren
                .ToDictionary(x => x.AttachmentId, x => x.Count);

            var model = new StoredDateiModel {
                TotalCount = stats.TotalResults,
                Start = 0,
                Count = files.Count,
                Dateien = files.Select(x => new StoredDateiEintragModel {
                    Id = x.Id,
                    MimeType = x.MimeType,
                    AttachmentId = x.AttachmentId,
                    Name = x.Name,
                    Beschreibung = x.Beschreibung,
                    Bytes = x.Bytes,
                    UseCount = fileUsage[x.AttachmentId]
                }).OrderByDescending(x => x.UseCount)
            };

            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Files(FileUploadModel model) {
            //var dbCommands = DocumentSession.Advanced.DocumentStore.DatabaseCommands;

            if (model.File != null) {
                var key = Guid.NewGuid().ToString();

                // Entity Speichern
                var datei = new Datei {
                    Name = model.File.FileName,
                    Beschreibung = model.Beschreibung,
                    MimeType = model.File.ContentType,
                    Bytes = model.File.Length,
                    AttachmentId = key
                };

                DocumentSession.Store(datei);

                var metadata = new JObject();
                metadata["OriginalDateiName"] = model.File.FileName;
                metadata["ContentType"] = model.File.ContentType;
                //dbCommands.PutAttachment(key, null, model.File.InputStream, metadata);

                DocumentSession.SaveChanges();

                DocumentSession.Query<Datei>()
                    .Customize(c => c.WaitForNonStaleResults())
                    .Take(0)
                    .ToArray();

                return RedirectToAction("Files");
            }

            return Json(null);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult Delete(string id) {
            var datei = DocumentSession.Load<Datei>(id.Replace("_","/"));
            if (datei == null) {
                return StatusCode((int)HttpStatusCode.NotFound, "Datei nicht gefunden");
            }

            var usage = DocumentSession.Query<FileUsageBySource.Result, FileUsageBySource>()
                .Where(x => x.AttachmentId == datei.AttachmentId)
                .ToList();

            var model = new FileDeleteModel {
                Id = id,
                Name = datei.Name,
                Usages = usage.Select(u => new FileUsageModel { 
                    DocumentId = u.AttachmentId,
                    DocumentName = u.DocumentName,
                    DocumentUrl = CreateUrl(u)
                }).ToList()
            };

            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult DeleteConfirmed(string id) {
            //var dbCommands = DocumentSession.Advanced.DocumentStore.DatabaseCommands;

            var datei = DocumentSession.Load<Datei>(id.Replace("_","/"));
            //dbCommands.DeleteAttachment(datei.AttachmentId, null); // TODO: Implementieren
            DocumentSession.Delete(datei);
            DocumentSession.SaveChanges();

            return RedirectToAction("Files");
        }

        private string CreateUrl(FileUsageBySource.Result usage) {
            switch (usage.DocumentType) {
                case "mitteilung":
                    return "/Aktuelles/Mitteilung/" + usage.DocumentReference.Replace("/", "_");
                case "seite":
                    return "/Content/Show/" + usage.DocumentReference;
                default:
                    return "/";
            }
        }
    }
}
