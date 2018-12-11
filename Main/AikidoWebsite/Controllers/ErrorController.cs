using System.Net;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AikidoWebsite.Web.Controllers
{
    public class ErrorController : Controller {

        //private ILogger Logger { get; }

        public ErrorController(/*ILogger logger*/)
        {
            //Logger = logger;
        }

        [HttpGet]
        [Route("/Error/404")]
        public ActionResult NotFound404() {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            ViewData["ErrorUrl"] = feature?.OriginalPath;

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View();
        }

        [HttpGet]
        [Route("/Error/401")]
        public ActionResult Unauthorized401() {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return View();
        }

        [HttpGet]
        [Route("/Error/{code:int}")]
        public ActionResult ServerError(int code) {
            //Logger.Fatal("Serverfehler aufgetreten");

            Response.StatusCode = code;
            return View();
        }

    }
}
