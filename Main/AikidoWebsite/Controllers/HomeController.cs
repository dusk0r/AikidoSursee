﻿using AikidoWebsite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AikidoWebsite.Data.Repositories;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Service.Services;
using Raven.Client;
using AikidoWebsite.Data.ValueObjects;

namespace AikidoWebsite.Controllers {

    public class HomeController : Controller {

        [Inject]
        public IUserManagementService UserManagementService { get; set; }

        [Inject]
        public IBenutzerRepository AccountRepository { get; set; }

        [Inject]
        public IDocumentSession DocumentSession { get; set; }

        [Inject]
        public ITestDataService TestDataService { get; set; }

        public ActionResult Index() {

            return RedirectToAction("Index", "Aktuelles");
        }

        public ActionResult CreateUser() {
            TestDataService.CreateTestUser();
            DocumentSession.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult CreatePosts() {
            TestDataService.CreateTestPosts();
            DocumentSession.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult CreateTermine() {
            var termin = new Termin {
                AutorId = "Benutzer/1",
                AutorName = "Christoph Enzmann",
                StartDatum = new DateTime(2013, 1, 22, 19, 0, 0),
                EndDatum = new DateTime(2013, 1, 22, 21, 0, 0),
                Titel = "Jorma Lyly in Sursee",
                Publikum = Publikum.Alle,
                Text = "Jorma Lyly in Sursee",
                ErstellungsDatum = new DateTime(2012, 12, 1, 0, 0, 0)
            };
            DocumentSession.Store(termin);

            var termin2 = new Termin {
                AutorId = "Benutzer/1",
                AutorName = "Christoph Enzmann",
                StartDatum = new DateTime(2013, 1, 24, 19, 0, 0),
                EndDatum = new DateTime(2013, 1, 24, 21, 0, 0),
                Titel = "Jorma Lyly in Sursee",
                Publikum = Publikum.Alle,
                Text = "Jorma Lyly in Sursee",
                ErstellungsDatum = new DateTime(2012, 12, 1, 0, 0, 0)
            };
            DocumentSession.Store(termin2);

            DocumentSession.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Impresum() {
            return View();
        }


        //public JsonResult AddTestAccounts() {
        //    //var userAccount = new AikidoWebsite.Data.Models.NewAccountModel {
        //    //    Name = "Hans User",
        //    //    EMail = "user@test.com",
        //    //    Password = "1234"
        //    //};
        //    //var adminAccount = new AikidoWebsite.Data.Models.NewAccountModel {
        //    //    Name = "Peter Admin",
        //    //    EMail = "admin@test.com",
        //    //    Password = "1234"
        //    //};
        //    //var user = AccountRepository.CreateAccount(userAccount);
        //    //var admin = AccountRepository.CreateAccount(adminAccount);
        //    //admin.SetAdmin(true);

        //    //Session.SaveChanges();

        //    return Json(true);
        //}
    }
}
