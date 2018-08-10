using System.Linq;
using AikidoWebsite.Data.Entities;
using Raven.Client.Documents.Indexes;

namespace AikidoWebsite.Data.Index
{

    public class ArchivedSeiteIndex : AbstractIndexCreationTask<ArchivedSeite, ArchivedSeiteIndex.Result>
    {

        public class Result
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Revision { get; set; }
        }

        public ArchivedSeiteIndex()
        {
            Map = archivedSeiten => from seite in archivedSeiten
                            select new
                            {
                                Id = seite.Id,
                                Name = seite.Seite.Name,
                                Revision = seite.Seite.Revision
                            };
        }
    }
}
