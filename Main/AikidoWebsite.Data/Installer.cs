using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.Security;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Raven.Client;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data {

    public class DataInstaller : IWindsorInstaller {

        public void Install(IWindsorContainer container, IConfigurationStore store) {
            // Identity
            container.Register(Component.For<IUserIdentity>().ImplementedBy<UserIdentity>().LifestyleTransient());

            IndexCreation.CreateIndexes(this.GetType().Assembly, container.Resolve<IDocumentStore>());

            // Patch Seiten
            // Todo, wieder entfernen
            using (var session = container.Resolve<IDocumentStore>().OpenSession()) {
                foreach (var seite in session.Query<Seite>()) {
                    foreach (var revision in seite.AlteRevisionen) {
                        session.Store(revision.Copy(), seite.Id + "/revision/" + revision.Revision);
                    }

                    seite.AlteRevisionen.Clear();
                }

                session.SaveChanges();
            }
        }
    }
}
