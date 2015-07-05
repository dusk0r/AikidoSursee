using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.Listener;
using AikidoWebsite.Data.Security;
using AikidoWebsite.Data.ValueObjects;
using AikidoWebsite.Web.Security;
using AikidoWebsite.Web.Service;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AikidoWebsite.Web {
    
    public class WebInstaller : IWindsorInstaller {

        public void Install(IWindsorContainer container, IConfigurationStore store) {

            // Logging
            container.AddFacility<LoggingFacility>(f => f.UseLog4Net().WithAppConfig());
            var logger = container.Resolve<ILogger>();

            // Store
            var documentStore = new DocumentStore { ConnectionStringName = "RavenDB" };
            documentStore.Initialize();
            documentStore.RegisterListener(new SeiteStoreListener());
            container.Register(Component.For<IDocumentStore>().Instance(documentStore).LifeStyle.Singleton);
            container.Register(Component.For<IDocumentSession>().UsingFactoryMethod(() => documentStore.OpenSession()).LifeStyle.PerWebRequest);
            logger.InfoFormat("Using Url {0} and Database {1}", documentStore.Url, documentStore.DefaultDatabase);

            // Clock
            container.Register(Component.For<IClock>().Instance(new Clock()).LifeStyle.Singleton);

            // Password Helper
            container.Register(Component.For<IPasswordHelper>().Instance(new PasswordHelper()).LifestyleSingleton());

            // Flickr
            container.Register(Component.For<FlickrService>().LifestyleSingleton());

            // Controller
            container.Register(
                AllTypes.FromThisAssembly()
                        .BasedOn(typeof(IController))
                        .LifestyleTransient()
            );

            // Container
            container.Register(Component.For<IWindsorContainer>().Instance(container).LifestyleSingleton());

            // Ensure Initial Data
            using (var session = documentStore.OpenSession()) {

                //session.Store(new Hinweis { Id = "default" });

                //var passwordHelper = container.Resolve<IPasswordHelper>();

                //var benutzer = new Benutzer[]{
                //    //CreateAdminBenutzer("Armin Müller", "am@arminsoft.ch", passwordHelper.CreateHashAndSalt("xxx")),
                //    //CreateAdminBenutzer("Georges Zahno", "georges.zahno@sunrise.ch", passwordHelper.CreateHashAndSalt("xxx")),
                //    //CreateAdminBenutzer("Marcel Schriber", "m_schriber@bluewin.ch", passwordHelper.CreateHashAndSalt("xxx")),
                //    //CreateAdminBenutzer("Priska Gut", "priska@gut.ch", passwordHelper.CreateHashAndSalt("xxx")),
                //    CreateAdminBenutzer("Robin Gubler", "robingubler@hotmail.com", passwordHelper.CreateHashAndSalt("xxx"))
                //};

                //foreach (var b in benutzer) {
                //    session.Store(b);
                //}

            //    if (!session.Query<Benutzer>().Any()) {
            //        
            //        //var password = passwordHelper.GeneratePassword(10);
            //        var password = "1234";

            //        session.Store(CreateAdminBenutzer(passwordHelper.CreateHashAndSalt(password)));
            //        session.SaveChanges();

            //        logger.FatalFormat("Create new Admin-User with password {0}", password);
            //    }

            //    logger.Debug("DB Setup OK");
            }
        }

        private static Benutzer CreateAdminBenutzer(string name, string email, string passwordHash) {
            var benutzer = new Benutzer {
                Name = name,
                EMail = email,
                IstAktiv = true,
                PasswortHash = passwordHash,
            };
            benutzer.Gruppen.Add(Gruppe.Benutzer);
            benutzer.Gruppen.Add(Gruppe.Admin);

            return benutzer;
        }
    }
}