using System.Net;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;

namespace AikidoWebsite.Web.Controllers
{
    public class ErrorController : Controller {

        private ILogger Logger { get; }

        public ErrorController(ILogger logger)
        {
            Logger = logger;
        }

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
