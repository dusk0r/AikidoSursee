using AikidoWebsite.Common;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AikidoWebsite.Service.Validator {

    public interface IValidatorService {

        /// <summary>
        /// Prüft das angegebene Entity mit allen verfügbaren Validatoren.
        /// Im Fehlerfall wird eine Exception vom Typ <c>ValidationException</c> geworfen.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void Validate<T>(T entity);

    }

    public class ValidatorService : IValidatorService {

        [Inject]
        public IWindsorContainer Container { get; set; }

        public void Validate<T>(T entity) {
            var validators = Container.ResolveAll<IValidator<T>>();
            var errors = new List<ValidationError>();

            foreach (var validator in validators) {
                errors.AddRange(validator.Validate(entity));
            }

            if (errors.Any()) {
                throw new ValidationException<T>(entity, errors);
            }
        }
    }
}
