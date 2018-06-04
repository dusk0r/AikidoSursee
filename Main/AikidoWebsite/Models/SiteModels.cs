namespace AikidoWebsite.Web.Models
{

    public class SeiteModel {
        public string Name { get; set; }
        public int Revision { get; set; }
        public string Markdown { get; set; }
        public string Html { get; set; }
        public bool Saved { get; set; }
    }

}
