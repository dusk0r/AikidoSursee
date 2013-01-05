using AikidoWebsite.Common;
using AikidoWebsite.Data.Security;
using AikidoWebsite.Web.Security;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Raven.Client;
using Raven.Client.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AikidoWebsite.Web {
    
    public class WebInstaller : IWindsorInstaller {

        public void Install(IWindsorContainer container, IConfigurationStore store) {

            // Store
            var documentStore = new DocumentStore { 
                Url = "http://localhost:8080/"
            };
            documentStore.Initialize();

            container.Register(Component.For<IDocumentStore>().Instance(documentStore).LifeStyle.Singleton);
            container.Register(Component.For<IDocumentSession>().UsingFactoryMethod(() => documentStore.OpenSession()).LifeStyle.PerWebRequest);

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

        }
    }
}