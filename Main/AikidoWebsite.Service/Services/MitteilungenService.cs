using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.Repositories;
using AikidoWebsite.Data.Security;
using AikidoWebsite.Data.ValueObjects;
using AikidoWebsite.Service.Validator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Service.Services {

    public interface IMitteilungenService {

        void CreateMitteilung(string titel, string text, Publikum publikum);

    }

    public class MitteilungenService : IMitteilungenService, IService {
        [Inject]
        public IClock Clock { get; set; }

        [Inject]
        public IUserIdentity UserIdentity { get; set; }

        [Inject]
        public IValidatorService Validator { get; set; }

        [Inject]
        public IMitteilungRepository MitteilungRepository { get; set; }

        public void CreateMitteilung(string titel, string text, Publikum publikum) {
            var mitteilung = new Mitteilung {
                Titel = titel,
                Text = text,
                Publikum = publikum,
                ErstelltAm = Clock.Now,
                AutorId = UserIdentity.Benutzer.Id,
                AutorName = UserIdentity.Benutzer.Name
            };

            Validator.Validate(mitteilung);

            MitteilungRepository.Store(mitteilung);
        }
    }
}
