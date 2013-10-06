using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Common {
    
    public static class HashCode {

        public static int FromObjects(params Object[] objects) {
            int hashCode = 19;

            unchecked {
                foreach (object o in objects) {
                    hashCode = hashCode * 41 + ((o == null) ? 0 : o.GetHashCode());
                }
            }

            return hashCode;
        }
    }
}
