using AikidoWebsite.Data.Repositories;
using AikidoWebsite.Data.Security;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Raven.Client;
using Raven.Client.Embedded;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data {

    public class DataInstaller : IWindsorInstaller {

        public void Install(IWindsorContainer container, IConfigurationStore store) {
            //// Store
            //var documentStore = new EmbeddableDocumentStore {
            //    DataDirectory = "Data",
            //    UseEmbeddedHttpServer = true,

            //}; // Todo, Config in XML

            //documentStore.Initialize();
            //container.Register(Component.For<IDocumentStore>().Instance(documentStore).LifeStyle.Singleton);
            //container.Register(Component.For<IDocumentSession>().UsingFactoryMethod(() => documentStore.OpenSession()).LifeStyle.PerWebRequest);

            // Repositories
            container.Register(
                AllTypes.FromThisAssembly()
                        .BasedOn(typeof(IRepository<>))
                        .WithServiceAllInterfaces()
                        .LifestyleTransient()
             );

            // Identity
            container.Register(Component.For<IUserIdentity>().ImplementedBy<UserIdentity>().LifestyleTransient());

        }
    }
}
