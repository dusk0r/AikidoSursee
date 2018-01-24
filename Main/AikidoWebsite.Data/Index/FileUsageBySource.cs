using System.Collections.Generic;
using System.Linq;
using AikidoWebsite.Data.Entities;
using Raven.Client.Documents.Indexes;

namespace AikidoWebsite.Data.Index
{

    /// <summary>
    /// Löst die Verwendung von Dateien auf
    /// </summary>
    public class FileUsageBySource : AbstractMultiMapIndexCreationTask<FileUsageBySource.Result> {

        public class Result {
            public string AttachmentId { get; set; }
            public string DocumentType { get; set; }
            public string DocumentReference { get; set; }
            public string DocumentName { get; set; }
        }

        public FileUsageBySource() {
            AddMap<Mitteilung>(mitteilungen => from mitteilung in mitteilungen
                                               from datei in mitteilung.DateiIds
                                               select new {
                                                   AttachmentId = datei,
                                                   DocumentType = "mitteilung",
                                                   DocumentReference = mitteilung.Id,
                                                   DocumentName = mitteilung.Titel
                                               });

            AddMap<Mitteilung>(mitteilungen => from mitteilung in mitteilungen
                                               from datei in (IEnumerable<string>)MetadataFor(mitteilung)["MarkupFiles"]
                                               select new {
                                                   AttachmentId = datei,
                                                   DocumentType = "mitteilung",
                                                   DocumentReference = mitteilung.Id,
                                                   DocumentName = mitteilung.Titel
                                               });

            AddMap<Seite>(seiten => from seite in seiten
                                    where !seite.Id.Contains("/revision/")
                                    from datei in (IEnumerable<string>)MetadataFor(seite)["MarkupFiles"]
                                    select new {
                                        AttachmentId = datei,
                                        DocumentType = "seite",
                                        DocumentReference = seite.Name,
                                        DocumentName = seite.Name
                                    });

            Store(x => x.DocumentName, FieldStorage.Yes);
            Store(x => x.DocumentReference, FieldStorage.Yes);
            Store(x => x.DocumentType, FieldStorage.Yes);
        }

    }
}
