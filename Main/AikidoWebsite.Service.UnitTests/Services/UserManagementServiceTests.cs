using AikidoWebsite.Common;
using AikidoWebsite.Data.Repositories;
using AikidoWebsite.Service.Services;
using AikidoWebsite.Service.Validator;
using AikidoWebsite.Data.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute;

namespace AikidoWebsite.Service.UnitTests.Services {
    
    [TestFixture]
    public class UserManagementServiceTests : ServiceTestBase<UserManagementService> {
        private IValidatorService Validator { get; set; }
        private IPasswordHelper PasswordHelper { get; set; }
        private IBenutzerRepository BenutzerRepository { get; set; }

        protected override void Prepare() {
            Validator = Mock<IValidatorService>();
            PasswordHelper = Mock<IPasswordHelper>();
            BenutzerRepository = Mock<IBenutzerRepository>();
        }

        [Test]
        public void CreateBenutzer_erzeugt_Account() {
            var benutzer = new Benutzer {
                Name = "Roi Danton",
                EMail = "roi@royal.olymp",
            };
            var password = "pass";
            PasswordHelper.SatisfiesComplexity(password).Returns(true);

            Service.CreateBenutzer(benutzer, password);

            Validator.Received().Validate(benutzer);
            PasswordHelper.Received().SatisfiesComplexity(password);
            BenutzerRepository.Received().Store(benutzer);
        }

        [Test]
        public void CreateBenutzer_mit_ungültigem_Passwort() {
            var benutzer = new Benutzer {
                Name = "Roi Danton",
                EMail = "roi@royal.olymp",
            };
            var password = "pass";
            PasswordHelper.SatisfiesComplexity(password).Returns(false);

            Assert.Throws<ValidationException<Benutzer>>(() => Service.CreateBenutzer(benutzer, password));

            BenutzerRepository.DidNotReceive().Store(benutzer);
        }

        [Test]
        public void CreateBenutzer_mit_ungültigen_Daten() {
            var benutzer = new Benutzer {
                Name = "Roi Danton",
                EMail = "",
            };
            var password = "pass";
            PasswordHelper.SatisfiesComplexity(password).Returns(true);
            Validator.When(x => x.Validate(benutzer)).Do(ci => { throw new ValidationException<Benutzer>("Fehler"); });

            Assert.Throws<ValidationException<Benutzer>>(() => Service.CreateBenutzer(benutzer, password));

            BenutzerRepository.DidNotReceive().Store(benutzer);
        }

        [Test]
        public void CreateBenutzer_mit_bereits_bestehender_EMail() {
            var benutzer = new Benutzer {
                Name = "Roi Danton",
                EMail = "roi@royal.olymp",
            };
            var password = "pass";
            PasswordHelper.SatisfiesComplexity(password).Returns(true);
            BenutzerRepository.FindByEmail("roi@royal.olymp").Returns(new Benutzer());

            Assert.Throws<ValidationException<Benutzer>>(() => Service.CreateBenutzer(benutzer, password));

            BenutzerRepository.DidNotReceive().Store(benutzer);
        }
    }
}
