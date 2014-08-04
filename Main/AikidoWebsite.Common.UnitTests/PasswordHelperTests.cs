using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace AikidoWebsite.Common.UnitTests {
    
    [TestFixture]
    public class PasswordHelperTests {
        public IPasswordHelper PasswordHelper { get; set; }

        [SetUp]
        public void SetUp() {
            PasswordHelper = new PasswordHelper();
        }


        [Test]
        public void CreatePassword_liefert_Passwort_mit_8_Stellen() {

            PasswordHelper.GeneratePassword(8).Should().NotBeNullOrWhiteSpace().And.HaveLength(8);
        }

        [Test]
        public void CreatePassword_liefert_Passwort_mit_12_Stellen() {

            PasswordHelper.GeneratePassword(12).Should().NotBeNullOrWhiteSpace().And.HaveLength(12);
        }

        [Test]
        public void HashAndSaltString_liefert_Hash() {
            var str = "Is' was, Doc?";

            PasswordHelper.CreateHashAndSalt(str).Should().NotBeNullOrWhiteSpace().And.HaveLength(60);
        }

        [Test]
        public void CheckPassword_prüft_Password() {
            var str = "Evelin S.";
            var hash = "$2a$10$lmetLWzYKI1TUFZX.XE1aeWKGw07UB3sNBj0GPdQ.Xv4nPXhhQH0K";

            PasswordHelper.VerifyPassword(str, hash).Should().BeTrue();
            PasswordHelper.VerifyPassword("abc", hash).Should().BeFalse();
            PasswordHelper.VerifyPassword("", hash).Should().BeFalse();
            PasswordHelper.VerifyPassword(null, hash).Should().BeFalse();
        }

        [Test]
        public void SatisfiesComplexity_Ungültige_Passwörter() {
            var passwords = new List<string> { 
                null,   // Null
                "",     // Leer
                " ",    // Nur Whitespaces
                "          ",   // Nur Whitespaces
                "abcefghijk",   // Nur Kleinbuchstaben
                "1bcefghijk",   // Kein Grossbuchstabe
                "Abcefghijk"    // Kein Sonderzeichen / Zahl
            };

            foreach (var password in passwords) {
                PasswordHelper.SatisfiesComplexity(password).Should().BeFalse();
            }
        }

        [Test]
        public void SatisfiesComplexity_Gültige_Passwörter() {
            var passwords = new List<string> { 
                "1Abcdefghijk",
                "%Abcdefghijk",
                "123456789A",
                "1234567B"
            };

            foreach (var password in passwords) {
                PasswordHelper.SatisfiesComplexity(password).Should().BeTrue();
            }
        }
    }
}
