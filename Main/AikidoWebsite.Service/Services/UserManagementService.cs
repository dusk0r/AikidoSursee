using AikidoWebsite.Common;
using AikidoWebsite.Data.Repositories;
using AikidoWebsite.Service.Validator;
using AikidoWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Service.Services {

    public interface IUserManagementService {

        void CreateBenutzer(Benutzer account, string password);
    }
    
    public class UserManagementService : IUserManagementService, IService {

        [Inject]
        public IValidatorService Validator { get; set; }

        [Inject]
        public IPasswordHelper PasswordHelper { get; set; }

        [Inject]
        public IBenutzerRepository BenutzerRepository { get; set; }

        public void CreateBenutzer(Benutzer benutzer, string password) {
            Check.NotNull(benutzer, "Kein Account");
            Check.StringHasValue(password, "Kein Passwort angegeben");

            if (!PasswordHelper.SatisfiesComplexity(password)) {
                throw new ValidationException<Benutzer>("Ungültiges Passwort");
            }

            // Hash Password
            benutzer.PasswortHash = PasswordHelper.CreateHashAndSalt(password);

            // Validate
            Validator.Validate(benutzer);

            if (BenutzerRepository.FindByEmail(benutzer.EMail) != null) {
                throw new ValidationException<Benutzer>("EMail Adresse bereits verwendet");
            }

            BenutzerRepository.Store(benutzer);
        }

    }
}
