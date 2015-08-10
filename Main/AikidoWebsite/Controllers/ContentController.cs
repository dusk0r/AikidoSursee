﻿using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AikidoWebsite.Web.Models;
using AikidoWebsite.Web.Extensions;
using AikidoWebsite.Data.ValueObjects;
using Raven.Client.Linq;
using Raven.Json.Linq;
using AikidoWebsite.Data.Index;
using System.Net;

namespace AikidoWebsite.Web.Controllers {

    public class ContentController : Controller {

        [Inject]
        public IClock Clock { get; set; }

        [Inject]
        public IDocumentSession DocumentSession { get; set; }

        public ActionResult Show(string id) {
            var article = DocumentSession.Query<Seite, AktuelleSeiteIndex>()
                .FirstOrDefault(a => a.Name == id);

            if (article != null) {
                return View(article);
            } else {
                Response.StatusCode = 404;
                Response.TrySkipIisCustomErrors = true;
                return View("ArtikelNichtGefunden", (object)id);
            }
        }

        [RequireGruppe(Gruppe.Admin)]
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
                .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                .ToList();

            return View(model);
        }

        [RequireGruppe(Gruppe.Admin)]
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
                .Customize(c => c.WaitForNonStaleResultsAsOfLastWrite())
                .Take(0)
                .ToArray();

            return Json(site.Id);
        }

        [HttpGet]
        public ActionResult File(string id) {
            var dbCommands = DocumentSession.Advanced.DocumentStore.DatabaseCommands;

            var attachment = dbCommands.GetAttachment(id);

            if (attachment == null) {
                return new HttpStatusCodeResult((int)System.Net.HttpStatusCode.NotFound);

            } else {
                var contentType = attachment.Metadata["ContentType"].ToString();

                return File(attachment.Data(), contentType);
            }
        }

        [RequireGruppe(Gruppe.Admin)]
        [HttpGet]
        [OutputCache(Duration=0)]
        public ActionResult Files() {
            // Todo: Paging
            RavenQueryStatistics stats = null;
            var files = DocumentSession.Query<Datei>()
                .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                .Statistics(out stats)
                .ToList();

            var fileUsage = DocumentSession.Query<FileUsageCountIndex.Result, FileUsageCountIndex>()
                .Where(x => x.AttachmentId.In(files.Select(f => f.AttachmentId)))
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

        [RequireGruppe(Gruppe.Admin)]
        [HttpPost]
        public ActionResult Files(FileUploadModel model) {
            var dbCommands = DocumentSession.Advanced.DocumentStore.DatabaseCommands;

            if (model.File != null) {
                var key = Guid.NewGuid().ToString();

                // Entity Speichern
                var datei = new Datei {
                    Name = model.File.FileName,
                    Beschreibung = model.Beschreibung,
                    MimeType = model.File.ContentType,
                    Bytes = model.File.ContentLength,
                    AttachmentId = key
                };

                DocumentSession.Store(datei);

                var metadata = new RavenJObject();
                metadata["OriginalDateiName"] = model.File.FileName;
                metadata["ContentType"] = model.File.ContentType;
                dbCommands.PutAttachment(key, null, model.File.InputStream, metadata);

                DocumentSession.SaveChanges();

                DocumentSession.Query<Datei>()
                    .Customize(c => c.WaitForNonStaleResultsAsOfLastWrite())
                    .Take(0)
                    .ToArray();

                return RedirectToAction("Files");
            }

            return Json(null);
        }

        [RequireGruppe(Gruppe.Admin)]
        [HttpGet]
        public ActionResult Delete(string id) {
            var datei = DocumentSession.Load<Datei>(id.Replace("_","/"));
            if (datei == null) {
                throw new HttpException((int)HttpStatusCode.NotFound, "Datei nicht gefunden");
            }

            var usage = DocumentSession.Query<FileUsageBySource.Result, FileUsageBySource>()
                .Where(x => x.AttachmentId == datei.AttachmentId)
                .ProjectFromIndexFieldsInto<FileUsageBySource.Result>()
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

        [RequireGruppe(Gruppe.Admin)]
        [HttpPost]
        public ActionResult DeleteConfirmed(string id) {
            var dbCommands = DocumentSession.Advanced.DocumentStore.DatabaseCommands;

            var datei = DocumentSession.Load<Datei>(id.Replace("_","/"));
            dbCommands.DeleteAttachment(datei.AttachmentId, null);
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
