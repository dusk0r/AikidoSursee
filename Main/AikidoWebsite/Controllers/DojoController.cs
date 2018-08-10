﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.Index;
using AikidoWebsite.Web.Extensions;
using AikidoWebsite.Web.Models;
using AikidoWebsite.Web.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Raven.Client.Documents.Session;

namespace AikidoWebsite.Web.Controllers
{
    public class DojoController : Controller {

        private IDocumentSession DocumentSession { get; }
        private FlickrService FlickrService { get; }
        private IHostingEnvironment HostingEnvironment { get; }

        public DojoController(IDocumentSession documentSession, FlickrService flickrService, IHostingEnvironment env)
        {
            DocumentSession = documentSession;
            FlickrService = flickrService;
            HostingEnvironment = env;
        }

        [HttpGet]
        public async Task<ActionResult> Bilder() {
            var gallerien = await FlickrService.ListPhotosetsAsync();
            var models = gallerien.Select(x => new PhotoSetModel {
                Titel = x.Title,
                Beschreibung = x.Description,
                ThumbnailUrl = x.PhotosetSquareThumbnailUrl,
                PhotosetId = x.PhotosetId,
                CreationDate = x.DateCreated,
                Link = x.Url
            }).OrderByDescending(x => x.CreationDate)
            .ToList();

            // Default Gallery
            var defaultGallery = new PhotoSetModel {
                Titel = "Das Dojo",
                Beschreibung = "Bilder vom Dojo Sursee",
                ThumbnailUrl = "/Content/images/dojo/dojo3p.jpg",
                PhotosetId = "sursee",
                CreationDate = new DateTime(2012, 11, 17),
                Link = @"http://www.aikido-sursee.ch/Dojo/Bilder" // TODO: Dynamisch machen
            };
            models = new PhotoSetModel[] { defaultGallery }.Union(models).ToList();

            return View(models);
        }

        [HttpGet]
        public async Task<JsonResult> ListBilder(string id) {
            IEnumerable<BildModel> models = null;

            if (id == "sursee") {
                models = Directory.GetFiles(Path.Combine(HostingEnvironment.WebRootPath, "~/Content/images/dojo"))
                    .Select(s => Path.GetFileName(s))
                    .Where(s => s.EndsWith(".jpg") && !s.Contains("p.jpg"))
                    .Select(x => new BildModel {
                        Titel = x,
                        Beschreibung = "",
                        ImageURL = "/Content/images/dojo/" + x,
                        Link = $@"{HttpContext.GetBaseUrl()}Dojo/Bilder"
                    });
            } else {
                models = (await FlickrService.ListPhotosAsync(id))
                    .Select(x => new BildModel {
                        Titel = x.Title,
                        Beschreibung = x.Description,
                        ImageURL = x.LargeUrl,
                        Link = x.WebUrl
                    });
            }

            return Json(models);
        }

        [HttpGet]
        public ActionResult Personen() {
            return View();
        }

        [HttpGet]
        public ActionResult Standort() {
            var article = DocumentSession.Load<Seite>("standort");

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
        [ValidateRecaptcha]
        public ActionResult Kontakt(KontaktModel model) {
            // Todo, Keys und Adresse in Config File auslagern
            //RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper("6LdHg_ASAAAAAEmzaiVNBGjMaYW8G--qwF6kwS-v");

            //if (String.IsNullOrEmpty(recaptchaHelper.Response)) {
            //    ModelState.AddModelError("capta", "Bitte geben Sie den Bestätigungscode an");
            //}
            //var recaptaResult = recaptchaHelper.VerifyRecaptchaResponse();
            //if (recaptaResult != RecaptchaVerificationResult.Success) {
            //    ModelState.AddModelError("capta", "Falscher Code");
            //}

            if (!ModelState.IsValid) {
                return View(model);
            }

            MailMessage mail = new MailMessage();
            mail.BodyEncoding = Encoding.UTF8;
            mail.From = new MailAddress("info@aikido-sursee.ch");
            mail.To.Add(new MailAddress("info@aikido-sursee.ch"));
            mail.Subject = String.Format("Anfrage von {0}: {1}", FilterSubjectChars(model.Name).Limit(10), FilterSubjectChars(model.Bemerkung).Limit(20));
            mail.Body = model.FormatText();

            SmtpClient smtpClient = new SmtpClient("localhost");
            smtpClient.Send(mail);

            return View("KontaktGesendet");
        }

        private static string FilterSubjectChars(string str) {
            return String.Join("", str.Where(c => Char.IsLetterOrDigit(c) || c == ' '));
        }

    }
}
