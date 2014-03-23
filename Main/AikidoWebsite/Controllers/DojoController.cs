using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Web.Models;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;

namespace AikidoWebsite.Web.Controllers {
    public class DojoController : Controller {

        [Inject]
        public IDocumentSession DocumentSession { get; set; }

        [HttpGet]
        public ActionResult Bilder() {
            var imageList = Directory.GetFiles(Server.MapPath("~/Content/images/dojo"))
                .Select(s => Path.GetFileName(s))
                .Where(s => s.EndsWith(".jpg") && !s.Contains("p.jpg"));

            return View(imageList);
        }

        [HttpGet]
        public ActionResult Personen() {
            return View();
        }

        [HttpGet]
        public ActionResult Standort() {
            var article = DocumentSession.Query<Seite>().SingleOrDefault(a => a.Name == "standort");

            if (article == null) {
                article = new Seite { WikiCreole = "" };
            }

            return View(article);
        }

        [HttpGet]
        public ActionResult Kontakt() {
            var model = new KontaktModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Kontakt(KontaktModel model) {
            // Todo, Keys und Adresse in Config File auslagern
            RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper("6LdHg_ASAAAAAEmzaiVNBGjMaYW8G--qwF6kwS-v");

            if (String.IsNullOrEmpty(recaptchaHelper.Response)) {
                ModelState.AddModelError("capta", "Bitte geben Sie den Bestätigungscode an");
            }
            var recaptaResult = recaptchaHelper.VerifyRecaptchaResponse();
            if (recaptaResult != RecaptchaVerificationResult.Success) {
                ModelState.AddModelError("capta", "Falscher Code");
            }

            if (!ModelState.IsValid) {
                return View(model);
            }

            MailMessage mail = new MailMessage();
            mail.BodyEncoding = Encoding.UTF8;
            mail.From = new MailAddress("info@aikido-sursee.ch");
            mail.To.Add(new MailAddress("info@aikido-sursee.ch"));
            mail.Subject = String.Format("Anfrage von {0}: {1}", model.Name.Limit(10), model.Bemerkung.Limit(20));
            mail.Body = model.FormatText();

            SmtpClient smtpClient = new SmtpClient("localhost");
            smtpClient.Send(mail);

            return View("KontaktGesendet");
        }

    }
}
