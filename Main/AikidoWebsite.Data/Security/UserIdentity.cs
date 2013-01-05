using AikidoWebsite.Common;
using AikidoWebsite.Data.Repositories;
using AikidoWebsite.Data.ValueObjects;
using AikidoWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace AikidoWebsite.Data.Security {

    public class UserIdentity : IUserIdentity {

        [Inject]
        public IBenutzerRepository BenutzerRepository { get; set; }

        public Benutzer Benutzer {
            get {
                if (!Thread.CurrentPrincipal.Identity.IsAuthenticated) {
                    return null;
                }

                var email = Thread.CurrentPrincipal.Identity.Name;
                return BenutzerRepository.FindByEmail(email);
            }
        }

        public bool IsAdmin {
            get {
                return (this.Benutzer != null && this.Benutzer.Gruppen.Contains(Gruppe.Admin));
            }
        }

    }
}