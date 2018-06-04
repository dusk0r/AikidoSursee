//using AikidoWebsite.Common;
//using AikidoWebsite.Data;
//using AikidoWebsite.Data.Security;
//using AikidoWebsite.Models;
//using AikidoWebsite.Web.Security;
//using AikidoWebsite.Web.Windsor;
//using Castle.Facilities.Logging;
//using Castle.MicroKernel.Registration;
//using Castle.Windsor;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Routing;
//using System.Web.Security;

//namespace AikidoWebsite.Web {
//    // Hinweis: Anweisungen zum Aktivieren des klassischen Modus von IIS6 oder IIS7 
//    // finden Sie unter "http://go.microsoft.com/?LinkId=9394801".

//    public class MvcApplication : System.Web.HttpApplication {
//        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
//            filters.Add(new HandleErrorAttribute());
//        }

//        public static void RegisterRoutes(RouteCollection routes) {
//            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

//            routes.MapRoute(
//                "Default", // Routenname
//                "{controller}/{action}/{id}", // URL mit Parametern
//                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameterstandardwerte
//            );

//            routes.MapRoute(
//                 "Value", // Routenname
//                 "{controller}/{action}/{id}/{value}", // URL mit Parametern
//                 new { controller = "Home", action = "Index", id = UrlParameter.Optional, value = UrlParameter.Optional }
//            );

//        }

//        protected void Application_Start() {
//            AreaRegistration.RegisterAllAreas();

//            RegisterGlobalFilters(GlobalFilters.Filters);
//            RegisterRoutes(RouteTable.Routes);

//            InitalizeWindsor();
//        }

//        public void InitalizeWindsor() {
//            var container = new WindsorContainer();

//            container.Install(
//                new WebInstaller(),
//                new DataInstaller()
//            );

//            // Controller Factory
//            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

//            // Inject Membership-/Roleprovider
//            container.Inject(Membership.Provider);
//            container.Inject(Roles.Provider);
//        }
//    }
//}