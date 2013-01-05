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

    public class TerminRepository : BaseRepository<Termin>, ITerminRepository {

        [Inject]
        public IDocumentSession Session { get; set; }


    }
}
