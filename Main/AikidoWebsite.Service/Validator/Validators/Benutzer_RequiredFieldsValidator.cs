using AikidoWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AikidoWebsite.Service.Validator.Validators {
    
    public class Benutzer_RequiredFieldsValidator : AbstractValidator<Benutzer> {

        public override IEnumerable<ValidationError> Validate(Benutzer benutzer) {
            var errors = ValidationError.CreateValidationList();

            if (String.IsNullOrWhiteSpace(benutzer.Name)) {
                errors.Add(CreateError("Ungültiger Name", x => x.Name));
            }

            // Todo, prüfe email adresse
            if (String.IsNullOrWhiteSpace(benutzer.EMail)) {
                errors.Add(CreateError("Ungültige EMail Adresse", x => x.EMail));
            }

            if (benutzer.PasswortHash == null || benutzer.PasswortHash.Length < 40) {
                errors.Add(CreateError("Kein Passwort vorhanden", x => x.PasswortHash));
            }

            return errors;
        }
    }
}
