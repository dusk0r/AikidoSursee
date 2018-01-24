using System.Collections.Generic;
using System.Linq;
using AikidoWebsite.Data.Entities;
using Raven.Client.Documents.Indexes;

namespace AikidoWebsite.Data.Index
{

    /// <summary>
    /// Zählt die Verwendung von Dateien
    /// </summary>
    public class FileUsageCountIndex : AbstractMultiMapIndexCreationTask<FileUsageCountIndex.Result> {

        public class Result {
            public string AttachmentId { get; set; }
            public int Count { get; set; }
        }

        public FileUsageCountIndex() {
            AddMap<Datei>(dateien => from datei in dateien
                                     select new {
                                         AttachmentId = datei.AttachmentId,
                                         Count = 0
                                     });

            AddMap<Mitteilung>(mitteilungen => from mitteilung in mitteilungen
                                               from datei in mitteilung.DateiIds
                                               select new {
                                                   AttachmentId = datei,
                                                   Count = 1
                                               });

            AddMap<Mitteilung>(mitteilungen => from mitteilung in mitteilungen
                                               from datei in (IEnumerable<string>)MetadataFor(mitteilung)["MarkupFiles"]
                                               select new {
                                                   AttachmentId = datei,
                                                   Count = 1
                                               });

            AddMap<Seite>(seiten => from seite in seiten
                                    where !seite.Id.Contains("/revision/")
                                    from datei in (IEnumerable<string>)MetadataFor(seite)["MarkupFiles"]
                                    select new {
                                        AttachmentId = datei,
                                        Count = 1
                                    });

            Reduce = results => from result in results
                                group result by result.AttachmentId into g
                                select new {
                                    AttachmentId = g.Key,
                                    Count = g.Sum(x => x.Count)
                                };
        }

    }
}
