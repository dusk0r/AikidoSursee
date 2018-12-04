using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Models;
using AikidoWebsite.Web.Extensions;
using AikidoWebsite.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;

namespace AikidoWebsite.Controllers
{
    public class AccountController : Controller {

        private IDocumentSession DocumentSession { get; }

        public AccountController(IDocumentSession documentSession)
        {
            DocumentSession = documentSession;
        }

        [HttpGet]
        public ActionResult LogOnExternal(string id)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/"
            };

            switch (id)
            {
                case "google":
                    return new ChallengeResult(GoogleDefaults.AuthenticationScheme, properties);
                case "facebook":
                    return new ChallengeResult(FacebookDefaults.AuthenticationScheme, properties);
                case "twitter":
                    return new ChallengeResult(TwitterDefaults.AuthenticationScheme, properties);
                default:
                    throw new Exception("Unknown Scheme: " + id);
            }
        }

        [HttpPost]
        public async Task<ActionResult> LogOn([FromForm]LogOnModel model) {
            var benutzer = DocumentSession.Query<Benutzer>()
                .FirstOrDefault(b => b.EMail == model.UserName);

            if (benutzer != null && benutzer.IstAktiv && benutzer.CheckPassword(model.Password))
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.NameIdentifier, benutzer.EMail),
                    new Claim(ClaimTypes.Name, benutzer.Name),
                    new Claim(ClaimTypes.Email, benutzer.EMail)
                };
                foreach (var role in benutzer.Gruppen)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.ToLowerInvariant()));
                }
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(principal, new AuthenticationProperties { IsPersistent = model.RememberMe });

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("UserName", "Benutzer nicht gefunden oder Passwort falsch");

            return View(model);
        }

        public async Task<ActionResult> LogOff() {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult ChangePassword() {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model) {
            var benutzer = DocumentSession.Query<Benutzer>().First(b => b.EMail.Equals(User.Identity.GetEmailAddress()));

            if (!benutzer.CheckPassword(model.CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "Altes Passwort ist falsch");
            } else if (!String.Equals(model.NewPassword, model.ConfirmPassword))
            {
                ModelState.AddModelError("ConfirmPassword", "Passwörter stimmen nicht überein");
            } else if (String.IsNullOrEmpty(model.NewPassword) || model.NewPassword.Length < 8)
            {
                ModelState.AddModelError("NewPassword", "Passwort ist zu kurz");
            } else
            {
                benutzer.SetPassword(model.NewPassword);
                DocumentSession.SaveChanges();
                return RedirectToAction("ChangePasswordSuccess");
            }

            return View(model);
        }

        public ActionResult ChangePasswordSuccess() {
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult ListUsers() {
            var benutzer = DocumentSession.Query<Benutzer>()
                .Take(1024)
                .Select(BenutzerModel.CreateBenutzer)
                .ToList();

            return View(benutzer);
        }
    }
}
