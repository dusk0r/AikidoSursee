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

        //private IAccountService AccountService { get { return IoC.Resolve<IAccountService>(); } }

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

            // Wurde dieser Punkt erreicht, ist ein Fehler aufgetreten; Formular erneut anzeigen.
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff() {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword() {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model) {
            if (ModelState.IsValid) {

                // In bestimmten Fehlerszenarien löst ChangePassword eine Ausnahme aus,
                // anstatt \"false\" zurückzugeben.
                bool changePasswordSucceeded;
                try {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
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

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess() {
            return View();
        }

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

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus) {
            // Eine vollständige Liste mit Statuscodes finden Sie unter http://go.microsoft.com/fwlink/?LinkID=177550
            //.
            switch (createStatus) {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Der Benutzername ist bereits vorhanden. Geben Sie einen anderen Benutzernamen ein.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Für diese E-Mail-Adresse ist bereits ein Benutzername vorhanden. Geben Sie eine andere E-Mail-Adresse ein.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Das angegebene Kennwort ist ungültig. Geben Sie einen gültigen Kennwortwert ein.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Die angegebene E-Mail-Adresse ist ungültig. Überprüfen Sie den Wert, und wiederholen Sie den Vorgang.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Die angegebene Kennwortabrufantwort ist ungültig. Überprüfen Sie den Wert, und wiederholen Sie den Vorgang.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Die angegebene Kennwortabruffrage ist ungültig. Überprüfen Sie den Wert, und wiederholen Sie den Vorgang.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Der angegebene Benutzername ist ungültig. Überprüfen Sie den Wert, und wiederholen Sie den Vorgang.";

                case MembershipCreateStatus.ProviderError:
                    return "Vom Authentifizierungsanbieter wurde ein Fehler zurückgegeben. Überprüfen Sie die Eingabe, und wiederholen Sie den Vorgang. Sollte das Problem weiterhin bestehen, wenden Sie sich an den zuständigen Systemadministrator.";

                case MembershipCreateStatus.UserRejected:
                    return "Die Benutzererstellungsanforderung wurde abgebrochen. Überprüfen Sie die Eingabe, und wiederholen Sie den Vorgang. Sollte das Problem weiterhin bestehen, wenden Sie sich an den zuständigen Systemadministrator.";

                default:
                    return "Unbekannter Fehler. Überprüfen Sie die Eingabe, und wiederholen Sie den Vorgang. Sollte das Problem weiterhin bestehen, wenden Sie sich an den zuständigen Systemadministrator.";
            }
        }
        #endregion
    }
}
