using AikidoWebsite.Common;
using AikidoWebsite.Service.Validator;
using AikidoWebsite.Web.Models;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AikidoWebsite.Web.Extensions;
using AikidoWebsite.Data.ValueObjects;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data;

namespace AikidoWebsite.Web.Controllers {
    public class AikidoInfosController : Controller {

        [Inject]
        public IDocumentSession DocumentSession { get; set; }

        [Inject]
        public IValidatorService ValidatorService { get; set; }

        [HttpGet]
        public ActionResult Faq() {
            var model = new FaqModel {
                IsAdmin = User.IsInGroup(Gruppe.Admin),
                Entries = DocumentSession.Query<FaqEintrag>()
            };
            return View(model);
        }

        [HttpPost]
        [RequireGruppe(Gruppe.Admin)]
        public JsonResult Faq(IEnumerable<FaqEintrag> entries) {
            foreach (var entry in entries) {
                ValidatorService.Validate(entry);
                DocumentSession.Store(entry);
            }
            DocumentSession.SaveChanges();

            return Json(true);
        }

        public ActionResult Glossar() {
            var model = new GlossarModel {
                IsAdmin = User.IsInGroup(Gruppe.Admin),
                Entries = DocumentSession.Query<GlossarEintrag>().OrderBy(g => g.Begriff)
            };
            return View(model);
        }

        [HttpPost]
        [RequireGruppe(Gruppe.Admin)]
        public JsonResult Glossar(IEnumerable<GlossarEintrag> entries) {
            foreach (var entry in entries) {
                ValidatorService.Validate(entry);
                DocumentSession.Store(entry);
            }
            DocumentSession.SaveChanges();

            return Json(true);
        }

        public ActionResult Techniken() {
            return View();
        }

    }
}
