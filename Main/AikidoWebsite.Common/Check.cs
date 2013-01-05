using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Common {

    public static class Check {

        public static T NotNull<T>(T value, string message = "Wert ist null") {
            if (value == null) {
                throw new ArgumentNullException(message);
            }

            return value;
        }

        public static string StringHasValue(string value, string message = "String ist null oder leer") {
            if (String.IsNullOrWhiteSpace(value)) {
                throw new ArgumentException(message);
            }

            return value;
        }

        public static void Argument(bool predicate, string message = "Ungültiges Argument") {
            if (!predicate) {
                throw new ArgumentException(message);
            }
        }
    }
}
