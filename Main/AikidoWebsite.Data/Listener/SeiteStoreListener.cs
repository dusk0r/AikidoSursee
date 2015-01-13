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

            if (seite != null) {
                var linkMatches = CreoleLink.Matches(seite.WikiCreole ?? "");
                var imageMatches = CreoleImage.Matches(seite.WikiCreole ?? "");

                var links = linkMatches.Cast<Match>().Where(m => m.Success).Select(m => m.Groups[2]).Select(g => g.Value);
                var images = imageMatches.Cast<Match>().Where(m => m.Success).Select(m => m.Groups[2]).Select(g => g.Value);

                metadata.Add("MarkupLinks", new RavenJArray(links));
                metadata.Add("MarkupImages", new RavenJArray(images));

                return true;
            }

            return false;
        }

        public void AfterStore(string key, object entityInstance, Raven.Json.Linq.RavenJObject metadata) {
            
        }


    }
}
