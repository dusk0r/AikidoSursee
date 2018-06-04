using System;
using System.Linq;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.ValueObjects;
using AikidoWebsite.Models;
using AikidoWebsite.Web.Extensions;
using AikidoWebsite.Web.Models;
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

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl) {
            if (ModelState.IsValid) {
                // TODO: Implementieren
                //if (Membership.ValidateUser(model.UserName, model.Password)) {
                //    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                //    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                //        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\")) {
                //        return Redirect(returnUrl);
                //    } else {
                //        return RedirectToAction("Index", "Home");
                //    }
                //} else {
                //    ModelState.AddModelError("", "Der angegebene Benutzername oder das angegebene Kennwort ist ungültig.");
                //}
            }

            return View(model);
        }

        public ActionResult LogOff() {
            //FormsAuthentication.SignOut();
            //await HttpContext.Authentication.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult ChangePassword() {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model) {
            // TODO: Implementieren
            //if (ModelState.IsValid) {

            //    bool changePasswordSucceeded;
            //    try {
            //        MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
            //        changePasswordSucceeded = currentUser.ChangePassword(null, model.NewPassword);
            //    } catch (Exception) {
            //        changePasswordSucceeded = false;
            //    }

            //    if (changePasswordSucceeded) {
            //        return RedirectToAction("ChangePasswordSuccess");
            //    } else {
            //        ModelState.AddModelError("", "Konnte das Passwort nicht ändern.");
            //    }
            //}

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

        //[Authorize(Roles = "Admin")]
        //public ActionResult AddUser(NewAccountModel newAccount) {
        //    if (ModelState.IsValid) {

        //    }

        //    return View();
        //}
    }
}
