using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;

namespace AikidoWebsite.Controllers
{

    public class HomeController : Controller {

        public IDocumentSession DocumentSession { get; }

        public HomeController(IDocumentSession documentSession)
        {
            DocumentSession = documentSession;
        }

        public ActionResult Index() {

            return RedirectToAction("Index", "Aktuelles");
        }

    }
}
