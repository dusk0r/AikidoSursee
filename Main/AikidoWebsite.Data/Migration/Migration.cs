using System.IO;
using System.Linq;
using AikidoWebsite.Data.Entities;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace AikidoWebsite.Data.Migration
{
    public class Migration
    {
        public static void DoMigration(IDocumentStore store)
        {
            var version = GetVersion(store);

            switch (version)
            {
                case 0:
                    DoMigrationTo1(store);
                    goto case 1;
                case 1:
                    DoMigrationTo2(store);
                    break;
                case 2:
                    DoMigrationTo3(store);
                    break;
            }
        }

        /// <summary>
        /// Seite aktualisieren
        /// </summary>
        private static void DoMigrationTo1(IDocumentStore store)
        {
            using (var session = store.OpenSession())
            {
                var query = session.Query<Seite>();
                var stream = session.Advanced.Stream(query);

                while (stream.MoveNext())
                {
                    if (stream.Current.Id.Contains("revision"))
                    {
                        session.Store(new ArchivedSeite { Seite = stream.Current.Document.Copy() });
                    }
                    else
                    {
                        var seite = stream.Current.Document.Copy();
                        seite.Id = "seites/" + seite.Name;
                        session.Store(seite);
                    }
                    session.Delete(stream.Current.Id);
                }

                session.Store(new DatabaseVersion { Version = 1 });
                session.SaveChanges();
            }
        }

        /// <summary>
        /// Seite aktualisieren
        /// </summary>
        private static void DoMigrationTo2(IDocumentStore store)
        {
            string attachmentDir = @"C:\Temp\Aikdo-Backup-Restore\Aikido-Backup-18-12-10";

            using (var session = store.OpenSession())
            {
                var query = session.Query<Datei>();
                var stream = session.Advanced.Stream(query);

                while (stream.MoveNext())
                {
                    var attachmentFilePath = Path.Combine(attachmentDir, stream.Current.Document.AttachmentId);

                    if (File.Exists(attachmentFilePath))
                    {
                        var entity = new Datei
                        {
                            Id = "dateis/" + stream.Current.Document.AttachmentId,
                            AttachmentId = stream.Current.Document.AttachmentId,
                            Name = stream.Current.Document.Name,
                            Beschreibung = stream.Current.Document.Beschreibung,
                            MimeType = stream.Current.Document.MimeType,
                            Bytes = stream.Current.Document.Bytes
                        };
                        session.Store(entity);
                        session.Advanced.Attachments.Store(entity, "file", File.Open(attachmentFilePath, FileMode.Open));
                    }

                    session.Delete(stream.Current.Id);
                }

                var version = session.Load<DatabaseVersion>("version");
                version.Version = 2;
                session.SaveChanges();
            }
        }

        /// <summary>
        /// Metadaten aktualisieren
        /// </summary>
        private static void DoMigrationTo3(IDocumentStore store)
        {
            using (var session = store.OpenSession())
            {
                foreach (var seite in session.Query<Seite>().ToList())
                {
                    var metadata = session.Advanced.GetMetadataFor(seite);
                    metadata["forceUpdate"] = 1.ToString();
                }
                foreach (var mitteilung in session.Query<Mitteilung>().ToList())
                {
                    var metadata = session.Advanced.GetMetadataFor(mitteilung);
                    metadata["forceUpdate"] = 1.ToString();
                }

                session.Store(new DatabaseVersion { Version = 3 });
                session.SaveChanges();
            }
        }

        private static int GetVersion(IDocumentStore store)
        {
            using (var session = store.OpenSession())
            {
                return session.Load<DatabaseVersion>("version")?.Version ?? 0;
            }
        }
    }

    public class DatabaseVersion
    {
        public string Id { get; set; } = "version";
        public int Version { get; set; }
    }
}
