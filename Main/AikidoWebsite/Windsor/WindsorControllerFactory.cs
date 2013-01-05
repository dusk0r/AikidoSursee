using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AikidoWebsite.Web.Windsor {
    
    public class WindsorControllerFactory : DefaultControllerFactory {
        private IWindsorContainer Container { get; set; }

        public WindsorControllerFactory(IWindsorContainer container) {
            this.Container = container;
        }

        public override void ReleaseController(IController controller) {
            Container.Release(controller);
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType) {
            if (controllerType == null) {
                throw new HttpException(404, string.Format("The controller for path '{0}' could not be found.", requestContext.HttpContext.Request.Path));
            }

            return (IController)Container.Resolve(controllerType);
        }
    }
}