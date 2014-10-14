using AikidoWebsite.Data.Entities;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AikidoWebsite.Data.Index {

    public class AktuelleSeiteIndex : AbstractIndexCreationTask<Seite, AktuelleSeiteIndex.Result> {

        public class Result {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Revision { get; set; }
        }

        public AktuelleSeiteIndex() {
            Map = seiten => from seite in seiten
                              where !seite.Id.Contains("revision")
                              select new {
                                  Id = seite.Id,
                                  Name = seite.Name,
                                  Revision = seite.Revision
                              };
        }
    }
}
