using Castle.Windsor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AikidoWebsite.Common;
using Castle.MicroKernel.Registration;
using NSubstitute;

namespace AikidoWebsite.Service.UnitTests.Services {

    public abstract class ServiceTestBase<TService> where TService : new() {
        protected IWindsorContainer Container { get; set; }
        public TService Service { get; set; }

        [SetUp]
        public void InitTest() {
            Container = new WindsorContainer();

            Prepare();

            Service = new TService();
            Container.Inject(Service);
        }

        protected abstract void Prepare();

        protected void Register<T>(T instance) where T : class {
            Container.Register(Component.For<T>().Instance(instance).LifestyleSingleton());
        }

        protected T Mock<T>() where T : class {
            var mock = Substitute.For<T>();
            Register(mock);
            return mock;
        }
    }
}
