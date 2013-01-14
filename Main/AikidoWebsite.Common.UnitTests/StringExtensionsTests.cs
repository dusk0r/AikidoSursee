using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;

namespace AikidoWebsite.Common.UnitTests {

    [TestFixture]
    public class StringExtensionsTests {

        [Test]
        public void Limit_short_text() {
            "abc".Limit(10).Should().Be("abc");
        }

        [Test]
        public void Limit_long_text() {
            "abcdefghijklmnop".Limit(10).Should().Be("abcdefg...");
        }

        [Test]
        public void Limit_long_text_custom_replacement() {
            "abcdefghijklmnop".Limit(10, "").Should().Be("abcdefghij");
        }

        [Test]
        public void NullSave_with_null() {
            string str = null;
            str.NullSave().Should().NotBeNull();
        }

        [Test]
        public void NullSave_without_null() {
            "abc".NullSave().Should().Be("abc");
        }

        [Test]
        public void RemoveNewline_with_newlines() {
            "a\nb\rcd\n\ref\r\ng".RemoveNewline().Should().Be("abcdefg");
        }

    }
}
