using AikidoWebsite.Data.Entities;
using Raven.Client.Listeners;
using Raven.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AikidoWebsite.Data.Listener {
    
    public class SeiteStoreListener : IDocumentStoreListener {
        private static readonly Regex CreoleLink = new Regex(@"\[\[(.*\/)(.*)\]\]");
        private static readonly Regex CreoleImage = new Regex(@"{{(.*\/)(.*)}}");

        public bool BeforeStore(string key, object entityInstance, Raven.Json.Linq.RavenJObject metadata, Raven.Json.Linq.RavenJObject original) {
            var seite = entityInstance as Seite;
            var mitteilung = entityInstance as Mitteilung;

            if (metadata.ContainsKey("MarkupFiles")) {
                metadata.Remove("MarkupLinks");
            }

            if (seite != null) {
                metadata.Add("MarkupFiles", new RavenJArray(ExtractFiles(seite.WikiCreole)));
                return true;
            }

            if (mitteilung != null) {
                metadata.Add("MarkupFiles", new RavenJArray(ExtractFiles(mitteilung.Text)));
                return true;
            }

            return false;
        }

        public void AfterStore(string key, object entityInstance, Raven.Json.Linq.RavenJObject metadata) {
            
        }

        private static IEnumerable<string> ExtractFiles(string creole) {
            var linkMatches = CreoleLink.Matches(creole ?? "");
            var imageMatches = CreoleImage.Matches(creole ?? "");

            var links = linkMatches.Cast<Match>().Where(m => m.Success).Select(m => m.Groups[2]).Select(g => g.Value).Select(StripLinkId);
            var images = imageMatches.Cast<Match>().Where(m => m.Success).Select(m => m.Groups[2]).Select(g => g.Value);

            return links.Concat(images);
        }

        private static string StripLinkId(string link) {
            if (link.Contains('|')) {
                var parts = link.Split('|');
                return parts[0];
            } else {
                return link;
            }
        }


    }
}
