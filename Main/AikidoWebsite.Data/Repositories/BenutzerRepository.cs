using AikidoWebsite.Common;
using AikidoWebsite.Data.Models;
using AikidoWebsite.Data.ValueObjects;
using AikidoWebsite.Data.Entities;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.Repositories {

    public class BenutzerRepository : BaseRepository<Benutzer>, IBenutzerRepository {

        [Inject]
        public IDocumentSession TestSession { get; set; }

        public Benutzer FindByEmail(string email) {
            Check.StringHasValue(email, "email");

            return Session.Query<Benutzer>().Where(a => a.EMail == email).FirstOrDefault(); 
        }

    }
}
