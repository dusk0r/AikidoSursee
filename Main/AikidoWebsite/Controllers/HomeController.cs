using AikidoWebsite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AikidoWebsite.Data.Entities;
using Raven.Client;
using AikidoWebsite.Data.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace AikidoWebsite.Controllers {

    public class HomeController : Controller {


        [Inject]
        public IDocumentSession DocumentSession { get; set; }

        public ActionResult Index() {

            return RedirectToAction("Index", "Aktuelles");
        }

    }
}
