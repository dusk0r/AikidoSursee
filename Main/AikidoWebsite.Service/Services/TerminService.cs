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

    public interface ITerminService {

        void CreateTermin(string titel, string text, Publikum publikum, DateTime startDatum, DateTime? endDatum = null);

    }

    public class TerminService : ITerminService, IService {
        [Inject]
        public IClock Clock { get; set; }

        [Inject]
        public IUserIdentity UserIdentity { get; set; }

        [Inject]
        public IValidatorService Validator { get; set; }

        [Inject]
        public ITerminRepository TerminRepository { get; set; }

        public void CreateTermin(string titel, string text, Publikum publikum, DateTime startDatum, DateTime? endDatum = null) {
            var termin = new Termin {
                Titel = titel,
                Text = text,
                Publikum = publikum,
                StartDatum = startDatum,
                EndDatum = endDatum,
                AutorId = UserIdentity.Benutzer.Id,
                AutorName = UserIdentity.Benutzer.Name
            };

            Validator.Validate(termin);

            TerminRepository.Store(termin);
        }

    }
}
