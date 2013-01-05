using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;

namespace AikidoWebsite.Common.UnitTests {
    
    [TestFixture]
    public class ClockTests {
        public IClock Clock { get; set; }

        [SetUp]
        public void SetUp() {
            Clock = new Clock();
        }

        [Test]
        public void Now_ist_jetzt() {

            Clock.Now.Subtract(DateTime.Now).Should().BeLessThan(TimeSpan.FromSeconds(1));
        }
    }
}
