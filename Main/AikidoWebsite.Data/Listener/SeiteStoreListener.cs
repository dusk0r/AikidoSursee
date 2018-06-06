using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AikidoWebsite.Data.Entities;
using Raven.Client.Documents.Session;

namespace AikidoWebsite.Data.Listener
{

    public static class SeiteStoreListener
    {
        private static readonly Regex CreoleLink = new Regex(@"\[\[(.*\/)(.*)\]\]");
        private static readonly Regex CreoleImage = new Regex(@"{{(.*\/)(.*)}}");

        public static void BeforeStore(object sender, BeforeStoreEventArgs e)
        {
            if (e.DocumentMetadata.ContainsKey("MarkupFiles"))
            {
                e.DocumentMetadata.Remove("MarkupFiles");
            }

            switch (e.Entity)
            {
                case Seite seite:
                    e.DocumentMetadata.Add("MarkupFiles", ExtractFiles(seite.WikiCreole));
                    break;
                case Mitteilung mitteilung:
                    e.DocumentMetadata.Add("MarkupFiles", ExtractFiles(mitteilung.Text));
                    break;
            }
        }

        private static IEnumerable<string> ExtractFiles(string creole)
        {
            var linkMatches = CreoleLink.Matches(creole ?? "");
            var imageMatches = CreoleImage.Matches(creole ?? "");

            var links = linkMatches.Cast<Match>().Where(m => m.Success).Select(m => m.Groups[2]).Select(g => g.Value).Select(StripLinkId);
            var images = imageMatches.Cast<Match>().Where(m => m.Success).Select(m => m.Groups[2]).Select(g => g.Value);

            return links.Concat(images);
        }

        private static string StripLinkId(string link)
        {
            if (link.Contains('|'))
            {
                var parts = link.Split('|');
                return parts[0];
            }
            else
            {
                return link;
            }
        }

    }
}
