using System;
using System.Linq;
using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.Extensions;
using AikidoWebsite.Data.Index;
using AikidoWebsite.Web.Extensions;
using AikidoWebsite.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

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
        public ActionResult Show(string id)
        {
            var article = DocumentSession.Load<Seite>(DocumentSession.GetRavenName<Seite>(id));

            if (article != null)
            {
                return View(article);
            }
            else
            {
                Response.StatusCode = 404;
                //Response.TrySkipIisCustomErrors = true;
                return View("ArtikelNichtGefunden", (object)id);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult Edit() {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public JsonResult LoadSeiteEditModel(string id = null)
        {
            var article = DocumentSession.Load<Seite>(DocumentSession.GetRavenName<Seite>(id));

            var model = new SeiteModel
            {
                Name = id,
                Revision = article?.Revision ?? -1,
                Text = article?.WikiCreole ?? String.Empty,
                Html = article?.Html ?? String.Empty
            };

            return Json(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public JsonResult Edit([FromBody] SeiteModel model) {
            var article = DocumentSession.Load<Seite>(DocumentSession.GetRavenName<Seite>(model.Name));
            var benutzer = DocumentSession.Query<Benutzer>()
                .First(b => b.EMail == User.Identity.GetEmailAddress());

            if (article == null)
            {
                article = new Seite
                {
                    Revision = 0,
                    Name = model.Name
                };
            }
            else
            {
                // Alte Revision speichern
                DocumentSession.Store(new ArchivedSeite { Seite = article.Copy() });
            }

            //// Update
            article.ErstellungsDatum = Clock.Now;
            article.Autor = benutzer.Name;
            article.WikiCreole = model.Text;
            article.Revision += 1;

            DocumentSession.Store(article);
            DocumentSession.SaveChanges();

            return Json(article.Revision);
        }

        [HttpGet]
        [ResponseCache(Duration = 3600)]
        public ActionResult File(string id) {
            var datei = DocumentSession.Load<Datei>(DocumentSession.GetRavenName<Datei>(id));
            if (datei == null)
            {
                return StatusCode((int)System.Net.HttpStatusCode.NotFound);
            }

            var attachment = DocumentSession.Advanced.Attachments.Get(datei, "file");

            if (attachment == null) {
                return StatusCode((int)System.Net.HttpStatusCode.NotFound);
            } else {
                return File(attachment.Stream, datei.MimeType);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public JsonResult ListFiles(int start = 0, int perPage = 10, string search = null) {
            perPage = perPage > 20 ? 10 : perPage;
            QueryStatistics stats = null;

            var query = DocumentSession.Query<FileUsageCountIndex.Result, FileUsageCountIndex>()
                .Where(x => x.Name != null);
            if (!String.IsNullOrEmpty(search))
            {

                foreach (var term in search.Split(' '))
                {
                    // TODO: Beschreibung
                    var queryTerm = $"*{term}*";
                    query = query.Search(x => x.Name, queryTerm, options: SearchOptions.And);
                }
            };
            var files = query.Statistics(out stats)
                .OrderByDescending(x => x.Count)
                .Skip(start)
                .Take(perPage)
                .ToList();

            var model = new StoredDateiModel {
                TotalCount = stats.TotalResults,
                Start = start,
                Count = files.Count,
                Dateien = files.Select(x => new StoredDateiEintragModel {
                    Id = x.AttachmentId,
                    MimeType = x.MimeType,
                    Name = x.Name,
                    Beschreibung = x.Beschreibung,
                    Bytes = x.Bytes,
                    UseCount = x.Count
                })
            };

            return Json(model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult Files()
        {
            return View();
        }

        //[Authorize(Roles = "admin")]
        //[HttpPost]
        //public ActionResult Files(FileUploadModel model) {
        //    //var dbCommands = DocumentSession.Advanced.DocumentStore.DatabaseCommands;

            //    if (model.File != null) {
            //        var key = Guid.NewGuid().ToString();

            //        // Entity Speichern
            //        var datei = new Datei {
            //            Name = model.File.FileName,
            //            Beschreibung = model.Beschreibung,
            //            MimeType = model.File.ContentType,
            //            Bytes = model.File.Length,
            //            AttachmentId = key
            //        };

            //        DocumentSession.Store(datei);

            //        var metadata = new JObject();
            //        metadata["OriginalDateiName"] = model.File.FileName;
            //        metadata["ContentType"] = model.File.ContentType;
            //        //dbCommands.PutAttachment(key, null, model.File.InputStream, metadata);

            //        DocumentSession.SaveChanges();

            //        DocumentSession.Query<Datei>()
            //            .Customize(c => c.WaitForNonStaleResults())
            //            .Take(0)
            //            .ToArray();

            //        return RedirectToAction("Files");
            //    }

            //    return Json(null);
            //}

            //[Authorize(Roles = "admin")]
            //[HttpGet]
            //public ActionResult Delete(string id) {
            //    var datei = DocumentSession.Load<Datei>(id.Replace("_","/"));
            //    if (datei == null) {
            //        return StatusCode((int)HttpStatusCode.NotFound, "Datei nicht gefunden");
            //    }

            //    var usage = DocumentSession.Query<FileUsageBySource.Result, FileUsageBySource>()
            //        .Where(x => x.AttachmentId == datei.AttachmentId)
            //        .ToList();

            //    var model = new FileDeleteModel {
            //        Id = id,
            //        Name = datei.Name,
            //        Usages = usage.Select(u => new FileUsageModel { 
            //            DocumentId = u.AttachmentId,
            //            DocumentName = u.DocumentName,
            //            DocumentUrl = CreateUrl(u)
            //        }).ToList()
            //    };

            //    return View(model);
            //}

            //[Authorize(Roles = "admin")]
            //[HttpPost]
            //public ActionResult DeleteConfirmed(string id) {
            //    //var dbCommands = DocumentSession.Advanced.DocumentStore.DatabaseCommands;

            //    var datei = DocumentSession.Load<Datei>(id.Replace("_","/"));
            //    //dbCommands.DeleteAttachment(datei.AttachmentId, null); // TODO: Implementieren
            //    DocumentSession.Delete(datei);
            //    DocumentSession.SaveChanges();

            //    return RedirectToAction("Files");
            //}

        [Authorize(Roles = "admin")]
        [HttpPost]
        public string ParseCreole([FromBody] CreoleModel model)
        {
            return model?.Text?.CreoleToHtml();
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
