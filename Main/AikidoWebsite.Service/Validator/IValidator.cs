using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AikidoWebsite.Service.Validator {
    
    public interface IValidator<T> {

        IEnumerable<ValidationError> Validate(T entity);
    }
}
