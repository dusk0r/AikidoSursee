using AikidoWebsite.Data.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AikidoWebsite.Web.Extensions {
    
    [AttributeUsage(AttributeTargets.Method)]
    public class RequireGruppeAttribute : AuthorizeAttribute {
        public Gruppe Gruppe { get; private set; }

        public RequireGruppeAttribute(Gruppe gruppe) {
            this.Gruppe = gruppe;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext) {
            return System.Web.Security.Roles.Provider.IsUserInRole(httpContext.User.Identity.Name, Gruppe.ToString());
        }

    }
}