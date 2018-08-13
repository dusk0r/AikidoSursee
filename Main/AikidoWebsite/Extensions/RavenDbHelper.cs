using AikidoWebsite.Common;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AikidoWebsite.Web.Extensions {
    
    public static class RavenDbHelper {

        public static string GetRavenName<T>(this IDocumentSession documentSession, string id)
        {
            var collectionName = documentSession.Advanced.DocumentStore.Conventions.GetCollectionName(typeof(T)).ToLowerInvariant();
            return $"{collectionName}/{id}";
        }

        public static string GetPublicName(string id)
        {
            Check.StringHasValue(id, nameof(id));

            return id.Substring(id.IndexOf('/'));
        }
    }
}