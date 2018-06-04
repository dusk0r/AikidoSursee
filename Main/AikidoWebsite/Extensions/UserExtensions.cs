//using AikidoWebsite.Data.ValueObjects;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Principal;
//using System.Web;

//namespace AikidoWebsite.Web.Extensions {
    
//    public static class UserExtensions {

//        public static bool IsInGroup(this IPrincipal principal, params Gruppe[] gruppen) {
//            return gruppen.Any(g => principal.IsInRole(g.ToString()));
//        }
//    }
//}