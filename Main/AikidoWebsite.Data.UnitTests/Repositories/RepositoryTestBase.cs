using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Embedded;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AikidoWebsite.Common;
using NSubstitute;

namespace AikidoWebsite.Data.UnitTests.Repositories {
    
    public abstract class RepositoryTestBase<T> where T: new() {
        protected IWindsorContainer Container { get; set; }
        protected T Repository { get; set; }

        [SetUp]
        public void InitTest() {
            Container = new WindsorContainer();

            var store = CreateStore();
            Container.Register(Component.For<IDocumentSession>().UsingFactoryMethod(() => store.OpenSession()).LifestyleTransient());

            Prepare();

            Repository = new T();
            Container.Inject(Repository);

            using (var session = store.OpenSession()) {
                CreateData(session);
            }
        }

        protected abstract void Prepare();

        protected abstract void CreateData(IDocumentSession session);

        protected void Register<T>(T instance) where T : class {
            Container.Register(Component.For<T>().Instance(instance).LifestyleSingleton());
        }

        protected T Mock<T>() where T : class {
            var mock = Substitute.For<T>();
            Register(mock);
            return mock;
        }

        private static IDocumentStore CreateStore() {
            var documentStore = new EmbeddableDocumentStore {
                RunInMemory = true,
                UseEmbeddedHttpServer = true,
            };
            documentStore.Initialize();

            return documentStore;
        }

    }
}
