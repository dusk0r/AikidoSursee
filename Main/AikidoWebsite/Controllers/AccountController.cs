using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using AikidoWebsite.Models;
using AikidoWebsite.Common;

namespace AikidoWebsite.Controllers {
    public class AccountController : Controller {

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl) {
            if (ModelState.IsValid) {
                if (Membership.ValidateUser(model.UserName, model.Password)) {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\")) {
                        return Redirect(returnUrl);
                    } else {
                        return RedirectToAction("Index", "Home");
                    }
                } else {
                    ModelState.AddModelError("", "Der angegebene Benutzername oder das angegebene Kennwort ist ungültig.");
                }
            }

            return View(model);
        }

        public ActionResult LogOff() {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult ChangePassword() {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model) {
            if (ModelState.IsValid) {

                // In bestimmten Fehlerszenarien löst ChangePassword eine Ausnahme aus,
                // anstatt \"false\" zurückzugeben.
                bool changePasswordSucceeded;
                try {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(null, model.NewPassword);
                } catch (Exception) {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded) {
                    return RedirectToAction("ChangePasswordSuccess");
                } else {
                    ModelState.AddModelError("", "Das aktuelle Kennwort ist nicht korrekt, oder das Kennwort ist ungültig.");
                }
            }

            // Wurde dieser Punkt erreicht, ist ein Fehler aufgetreten; Formular erneut anzeigen.
            return View(model);
        }

        //public ActionResult ChangePasswordSuccess() {
        //    return View();
        //}

        //[Authorize(Roles = "Admin")]
        public ActionResult ListUsers() {
            //return View(AccountService.GetAll());
            return View();
        }

        //[Authorize(Roles = "Admin")]
        //public ActionResult AddUser(NewAccountModel newAccount) {
        //    if (ModelState.IsValid) {

        //    }

        //    return View();
        //}
    }
}
