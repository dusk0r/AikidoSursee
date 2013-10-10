using AikidoWebsite.Common;
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

namespace AikidoWebsite.Web.Controllers {

    public class ContentController : Controller {

        [Inject]
        public IClock Clock { get; set; }

        [Inject]
        public IDocumentSession DocumentSession { get; set; }

        public ActionResult Show(string id) {
            var article = DocumentSession.Query<Seite>().SingleOrDefault(a => a.Name == id);

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
            var article = DocumentSession.Query<Seite>().SingleOrDefault(a => a.Name == id);

            var model = new SeiteModel {
                Name = id,
                Revision = (article != null) ? article.Revision : 0,
                Markdown = (article != null) ? article.WikiCreole : "",
                Html = (article != null) ? article.Html : "",
                Saved = saved
            };

            return View(model);
        }

        [RequireGruppe(Gruppe.Admin)]
        [HttpPost]
        public JsonResult Edit(SeiteModel model) {
            var site = DocumentSession.Query<Seite>().SingleOrDefault(a => a.Name == model.Name);
            var benutzer = DocumentSession.Query<Benutzer>().First(b => b.EMail.Equals(User.Identity.Name));

            var oldRevisions = new HashSet<Seite>();

            if (site == null) {
                site = new Seite {
                    Revision = 0,
                    Name = model.Name
                };
            } else {
                // Revisionen aufräumen
                foreach (var revison in site.AlteRevisionen) {
                    oldRevisions.Add(revison);
                }
                oldRevisions.Add(new Seite { 
                    Name = site.Name,
                    Autor = site.Autor, 
                    ErstellungsDatum = site.ErstellungsDatum, 
                    Revision = site.Revision, 
                    WikiCreole = site.WikiCreole });
                site.AlteRevisionen = oldRevisions;
            }

            // Update
            site.ErstellungsDatum = Clock.Now;
            site.Autor = benutzer.Name;
            site.WikiCreole = model.Markdown;
            site.Revision += 1;

            DocumentSession.Store(site);
            DocumentSession.SaveChanges();

            return Json(site.Id);
        }

    }
}
