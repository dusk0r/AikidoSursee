namespace AikidoWebsite.Data.Entities
{
    public class ArchivedSeite : IEntity
    {
        public string Id { get; set; }

        public Seite Seite { get; set; }
    }
}
