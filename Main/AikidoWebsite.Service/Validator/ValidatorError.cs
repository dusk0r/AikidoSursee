using AikidoWebsite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AikidoWebsite.Service.Validator {
    
    public class ValidationError {
        public string PropertyName { get; private set; }
        public string ErrorText { get; private set; }

        public ValidationError(string errorText, string propertyName = null) {
            this.PropertyName = propertyName;
            this.ErrorText = Check.StringHasValue(errorText, "Kein Errortext");
        }

        public static IList<ValidationError> CreateValidationList() {
            return new List<ValidationError>();
        }

    }
}
