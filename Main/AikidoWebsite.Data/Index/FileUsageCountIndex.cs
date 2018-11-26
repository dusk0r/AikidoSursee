using System;
using System.Collections.Generic;
using System.Linq;
using AikidoWebsite.Data.Entities;
using Raven.Client.Documents.Indexes;

namespace AikidoWebsite.Data.Index
{

    /// <summary>
    /// Zählt die Verwendung von Dateien
    /// </summary>
    public class FileUsageCountIndex : AbstractMultiMapIndexCreationTask<FileUsageCountIndex.Result>
    {

        public class Result
        {
            public string AttachmentId { get; set; }
            public string Name { get; set; }
            public string Beschreibung { get; set; }
            public string MimeType { get; set; }
            public long Bytes { get; set; }
            public int Count { get; set; }
            public DateTime UploadDate { get; set; }
        }

        public FileUsageCountIndex()
        {
            AddMap<Datei>(dateien => from datei in dateien
                                     select new
                                     {
                                         AttachmentId = datei.AttachmentId,
                                         Name = datei.Name,
                                         Beschreibung = datei.Beschreibung,
                                         MimeType = datei.MimeType,
                                         Bytes = datei.Bytes,
                                         UploadDate = datei.UploadDate,
                                         Count = 0
                                     });

            AddMap<Mitteilung>(mitteilungen => from mitteilung in mitteilungen
                                               from datei in mitteilung.DateiIds
                                               select new
                                               {
                                                   AttachmentId = datei,
                                                   Name = default(string),
                                                   Beschreibung = default(string),
                                                   MimeType = default(string),
                                                   Bytes = 0,
                                                   UploadDate = DateTime.MinValue,
                                                   Count = 1
                                               });

            AddMap<Mitteilung>(mitteilungen => from mitteilung in mitteilungen
                                               from datei in (IEnumerable<string>)MetadataFor(mitteilung)["MarkupFiles"]
                                               select new
                                               {
                                                   AttachmentId = datei,
                                                   Name = default(string),
                                                   Beschreibung = default(string),
                                                   MimeType = default(string),
                                                   Bytes = 0,
                                                   UploadDate = DateTime.MinValue,
                                                   Count = 1
                                               });

            AddMap<Seite>(seiten => from seite in seiten
                                    from datei in (IEnumerable<string>)MetadataFor(seite)["MarkupFiles"]
                                    select new
                                    {
                                        AttachmentId = datei,
                                        Name = default(string),
                                        Beschreibung = default(string),
                                        MimeType = default(string),
                                        Bytes = 0,
                                        UploadDate = DateTime.MinValue,
                                        Count = 1
                                    });

            Reduce = results => from result in results
                                group result by result.AttachmentId into g
                                let datei = g.FirstOrDefault(x => x.Name != null)
                                select new
                                {
                                    AttachmentId = g.Key,
                                    Name = (datei != null) ? datei.Name : default(string),
                                    Beschreibung = (datei != null) ? datei.Beschreibung : default(string),
                                    MimeType = (datei != null) ? datei.MimeType : default(string),
                                    Bytes = g.Sum(x => x.Bytes),
                                    UploadDate = g.Max(x => x.UploadDate),
                                    Count = g.Sum(x => x.Count)
                                };
        }

    }
}
