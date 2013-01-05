using AikidoWebsite.Service.Validator.Validators;
using AikidoWebsite.Data.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace AikidoWebsite.Service.UnitTests.Validator.Validators {
    
    [TestFixture]
    public class BenutzerValidatorsTests {

        [Test]
        public void HasRequiredFields_mit_fehlenden_Feldern() {
            var validator = new Benutzer_RequiredFieldsValidator();
            var benutzer = new Benutzer();

            var res = validator.Validate(benutzer);

            res.Should().HaveCount(3);
        }

        [Test]
        public void HasRequiredField_ok() {
            var validator = new Benutzer_RequiredFieldsValidator();
            var benutzer = new Benutzer {
                Name = "Peter 123",
                EMail = "test@test.com",
                PasswortHash = String.Join("", Enumerable.Range(0, 50))
            };

            var res = validator.Validate(benutzer);

            res.Should().HaveCount(0);
        }

    }
}
