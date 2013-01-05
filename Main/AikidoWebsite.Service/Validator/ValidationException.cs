using AikidoWebsite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AikidoWebsite.Service.Validator {
    
    public class ValidationException<T> : Exception {
        private List<ValidationError> errors;
        
        public T Entity { get; private set; }
        public IList<ValidationError> Errors { get { return errors.AsReadOnly(); } }
        public override string Message {
            get {
                return "Es sind Fehler aufgetreten: " + String.Join(", ", errors);
            }
        }

        public ValidationException(T entity, IEnumerable<ValidationError> errors) {
            Check.NotNull(errors, "Keine Fehler übergeben");
            
            Entity = entity;
            errors = new List<ValidationError>(errors);
        }

        public ValidationException(string error) {
            Check.StringHasValue(error, "Kein Fehler angegeben");

            errors = new List<ValidationError> { new ValidationError(error) };
        }
    }
}
