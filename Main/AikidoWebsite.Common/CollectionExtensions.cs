using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Common {
    
    public static class CollectionExtensions {

        public static ISet<T> ToSet<T>(this IEnumerable<T> collection) {
            return new HashSet<T>(collection);
        }
    }
}
