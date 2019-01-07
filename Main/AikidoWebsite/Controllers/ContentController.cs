using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.Extensions;
using AikidoWebsite.Data.Index;
using AikidoWebsite.Web.Extensions;
using AikidoWebsite.Web.Models;
using AikidoWebsite.Web.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Raven.Client.Documents.Smuggler;

namespace AikidoWebsite.Web.Controllers
{

    public class ContentController : Controller {

        private IDocumentSession DocumentSession { get; }
        private IClock Clock { get; }
        private BackupTokenService BackupTokenService { get; }

        public ContentController(IDocumentSession documentSession, IClock clock, BackupTokenService backupTokenService)
        {
            DocumentSession = documentSession;
            Clock = clock;
            BackupTokenService = backupTokenService;
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
        public ActionResult Files()
        {
            return View();
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
                .OrderByDescending(x => x.UploadDate)
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
        public JsonResult GetFileUsage(string id) {
            var usage = DocumentSession.Query<FileUsageBySource.Result, FileUsageBySource>()
                .Where(x => x.AttachmentId == id)
                .ProjectInto<FileUsageBySource.Result>()
                .ToList();

            return Json(usage);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        public JsonResult DeleteConfirmed(string id) {
            var datei = DocumentSession.Load<Datei>(DocumentSession.GetRavenName<Datei>(id));
            DocumentSession.Delete(datei);
            DocumentSession.SaveChanges();

            return Json(true);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public JsonResult UploadFile([FromForm]IFormFile file, [FromForm]string bezeichnung)
        {
            var key = Guid.NewGuid().ToString();

            var datei = new Datei
            {
                Id = DocumentSession.GetRavenName<Datei>(key),
                Name = file.FileName,
                Beschreibung = bezeichnung,
                MimeType = file.ContentType,
                Bytes = file.Length,
                AttachmentId = key,
                UploadDate = Clock.Now
            };
            DocumentSession.Store(datei);

            using (var inputStream = file.OpenReadStream())
            {
                DocumentSession.Advanced.Attachments.Store(datei, "file", inputStream, file.ContentType);
                DocumentSession.SaveChanges();
            }

            return Json(datei.Id);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public string ParseCreole([FromBody] CreoleModel model)
        {
            return model?.Text?.CreoleToHtml();
        }

        [HttpGet]
        public async Task<ActionResult> DownloadBackup(string secret)
        {
            if (!BackupTokenService.CheckToken(secret))
            {
                return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }

            var path = Path.GetTempFileName();
            var operation = await DocumentSession.Advanced.DocumentStore.Smuggler.ExportAsync(new DatabaseSmugglerExportOptions
            {
                OperateOnTypes = DatabaseItemType.Documents
            }, path);
            await operation.WaitForCompletionAsync();

            var filename = $"Backup-Aikido-{Clock.Now.ToString("yyyy-MM-dd")}.ravendb";
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose);
            return File(fileStream: fs, contentType: System.Net.Mime.MediaTypeNames.Application.Octet, fileDownloadName: filename);
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
