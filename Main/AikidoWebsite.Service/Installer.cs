using AikidoWebsite.Service.Validator;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AikidoWebsite.Service {
    
    public class ServiceInstaller : IWindsorInstaller {

        public void Install(IWindsorContainer container, IConfigurationStore store) {
            
            // ValidationService
            container.Register(Component.For<IValidatorService>().ImplementedBy<ValidatorService>().LifestyleTransient());

            // Validators
            container.Register(
                AllTypes.FromThisAssembly()
                        .BasedOn(typeof(IValidator<>))
                        .WithServiceAllInterfaces()
                        .LifestyleTransient()
             );

            //// Services
            //container.Register(
            //    AllTypes.FromThisAssembly()
            //            .BasedOn(typeof(IService))
            //            .WithServiceAllInterfaces()
            //            .LifestyleTransient()
            // );
        }
    }
}
