using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.Models;
using AikidoWebsite.Data.Security;
using AikidoWebsite.Data.ValueObjects;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.Repositories {
    
    public class MitteilungRepository : BaseRepository<Mitteilung>, IMitteilungRepository {

        [Inject]
        public IUserIdentity UserIdentity { get; set; }

        public IEnumerable<Mitteilung> GetLatestMitteilungen(int start = 0, int count = Int32.MaxValue) {
            Check.Argument(start >= 0, "start muss grösser oder gleich 0 sein");
            Check.Argument(count >= 0, "mehr als ein BlogPost muss ausgewählt sein");

            return Session.Query<Mitteilung>().OrderByDescending(p => p.ErstelltAm).Skip(start).Take(count);
        }



    }
}
