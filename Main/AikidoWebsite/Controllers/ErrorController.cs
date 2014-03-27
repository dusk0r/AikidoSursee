using AikidoWebsite.Common;
using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AikidoWebsite.Web.Controllers {
    public class ErrorController : Controller {

        [Inject]
        public ILogger Logger { get; set; }

        public ActionResult NotFound() {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View();
        }

        public ActionResult Unauthorized() {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return View();
        }

        public ActionResult ServerError() {
            Logger.Fatal("Serverfehler aufgetreten");

            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return View();
        }

    }
}
