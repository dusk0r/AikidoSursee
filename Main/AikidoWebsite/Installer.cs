﻿using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.Security;
using AikidoWebsite.Data.ValueObjects;
using AikidoWebsite.Web.Security;
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

            // Store
            // Todo: Configuration
            var documentStore = new DocumentStore { 
                Url = "http://localhost:8080/",
            };
            documentStore.Initialize();
            // Todo: Configuration
            documentStore.DatabaseCommands.EnsureDatabaseExists("aikido");

            container.Register(Component.For<IDocumentStore>().Instance(documentStore).LifeStyle.Singleton);
            // Todo: Configuration
            container.Register(Component.For<IDocumentSession>().UsingFactoryMethod(() => documentStore.OpenSession("aikido")).LifeStyle.PerWebRequest);

            // Logging
            container.AddFacility<LoggingFacility>(f => f.UseLog4Net());

            // Clock
            container.Register(Component.For<IClock>().Instance(new Clock()).LifeStyle.Singleton);

            //// Identity
            //container.Register(Component.For<IUserIdentity>().Instance(new UserIdentity()).LifeStyle.Singleton);

            // Password Helper
            container.Register(Component.For<IPasswordHelper>().Instance(new PasswordHelper()).LifestyleSingleton());

            // Controller
            container.Register(
                AllTypes.FromThisAssembly()
                        .BasedOn(typeof(IController))
                        .LifestyleTransient()
            );

            // Container
            container.Register(Component.For<IWindsorContainer>().Instance(container).LifestyleSingleton());

            // Ensure Initial Data
            // Todo: Configuration
            using (var session = documentStore.OpenSession("aikido")) {
                var logger = container.Resolve<ILogger>();

                if (!session.Query<Benutzer>().Any()) {
                    var passwordHelper = container.Resolve<IPasswordHelper>();
                    //var password = passwordHelper.GeneratePassword(10);
                    var password = "1234";

                    session.Store(CreateAdminBenutzer(passwordHelper.CreateHashAndSalt(password)));
                    session.SaveChanges();

                    logger.FatalFormat("Create new Admin-User with password {0}", password);
                }

                logger.Debug("DB Setup OK");
            }

        }

        private static Benutzer CreateAdminBenutzer(string passwordHash) {
            var benutzer = new Benutzer {
                Name = "Admin",
                EMail = "admin@amigo-online.ch",
                IstAktiv = true,
                PasswortHash = passwordHash,
            };
            benutzer.Gruppen.Add(Gruppe.Benutzer);
            benutzer.Gruppen.Add(Gruppe.Admin);

            return benutzer;
        }
    }
}