using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace AikidoWebsite.Common.UnitTests {

    [TestFixture]
    public class CheckTests {

        [Test]
        public void NotNull_löst_bei_Null_Exception_aus() {

            Assert.Throws<ArgumentNullException>(() => Check.NotNull((String)null));
        }

        [Test]
        public void NotNull_gibt_den_gültigen_Wert_zurück() {
            var obj = new Object();

            Check.NotNull(obj).Should().BeSameAs(obj);
        }

        [Test]
        public void StringHasValue_wirft_Exception_bei_Null() {
            string str = null;

            Assert.Throws<ArgumentException>(() => Check.StringHasValue(str));
        }

        [Test]
        public void StringHasValue_wirft_Excepton_bei_leerem_String() {
            var str = "    ";

            Assert.Throws<ArgumentException>(() => Check.StringHasValue(str));
        }

        [Test]
        public void StringHasValue_liefert_string_zurück() {
            var str = "Hallo 123";

            Check.StringHasValue(str).Should().Be("Hallo 123");
        }

        [Test]
        public void Argument_wirft_Exception_wenn_Bedingung_ungültig() {

            Assert.Throws<ArgumentException>(() => Check.Argument(false));
        }

        [Test]
        public void Argument_machts_nichts_wenn_Bedingung_gültig() {

            Check.Argument(true);
        }
    }
}
