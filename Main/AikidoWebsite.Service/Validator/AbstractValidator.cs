using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AikidoWebsite.Service.Validator {
    
    public abstract class AbstractValidator<T> : IValidator<T> {

        public ValidationError CreateError(string errorText, Expression<Func<T, object>> propertySelector = null) {
            string propertyName = null;

            if (propertySelector != null) {
                var memberExpression = propertySelector.Body as MemberExpression;
                var pi = memberExpression.Member as PropertyInfo;
                propertyName = pi.Name;
            }

            return new ValidationError(errorText, propertyName);
        }

        public abstract IEnumerable<ValidationError> Validate(T entity);
    }
}
