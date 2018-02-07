using AikidoWebsite.Common;
using AikidoWebsite.Data.ValueObjects;
using AikidoWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Raven.Client;
using Raven.Client.Documents.Session;

namespace AikidoWebsite.Data.Security {

    public class UserIdentity : IUserIdentity {
        private IDocumentSession Session { get; set; }

        public UserIdentity(IDocumentSession session)
        {
            this.Session = session;
        }

        public Benutzer Benutzer {
            get {
                if (!Thread.CurrentPrincipal.Identity.IsAuthenticated) {
                    return null;
                }

                var email = Thread.CurrentPrincipal.Identity.Name;
                return Session.Query<Benutzer>().FirstOrDefault(b => b.EMail == email);
            }
        }

        public bool IsAdmin {
            get {
                return (this.Benutzer != null && this.Benutzer.Gruppen.Contains(Gruppe.Admin));
            }
        }

    }
}