using AikidoWebsite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AikidoWebsite.Web.Extensions {
    
    public static class RavenDbHelper {

        public static string EncodeDocumentId(string documentId) {
            Check.StringHasValue(documentId, "DocumentId");

            return documentId.Replace('/', '_');
        }

        public static string DecodeDocumentId(string encodedDocumentId) {
            Check.StringHasValue(encodedDocumentId, "DocumentId");

            return encodedDocumentId.Replace('_', '/');
        }
    }
}