using System.Collections.Generic;

namespace AikidoWebsite.Common
{

    public static class CollectionExtensions {

        public static ISet<T> ToSet<T>(this IEnumerable<T> collection) {
            return new HashSet<T>(collection);
        }
    }
}
