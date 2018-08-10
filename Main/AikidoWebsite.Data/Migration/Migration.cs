using AikidoWebsite.Data.Entities;
using Raven.Client.Documents;

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
                    break;
            }
        }

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
                    } else
                    {
                        var seite = stream.Current.Document.Copy();
                        seite.Id = "seites/" + seite.Name;
                        session.Store(seite);
                    }
                    session.Delete(stream.Current.Id);
                }

                session.Store(new DatabaseVersion {  Version = 1 });
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
