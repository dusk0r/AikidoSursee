using AikidoWebsite.Data.Repositories;
using AikidoWebsite.Data.Entities;
using NUnit.Framework;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using AikidoWebsite.Common;

namespace AikidoWebsite.Data.UnitTests.Repositories {

    [TestFixture]
    public class BenutzerRepositoryTests : RepositoryTestBase<BenutzerRepository> {

        protected override void Prepare() {
            Mock<IPasswordHelper>();
        }

        protected override void CreateData(IDocumentSession session) {
            var benutzer = new Benutzer {
                Name = "Barney Stinson",
                EMail = "barney@bro.com",
            };

            session.Store(benutzer);
            session.SaveChanges();
        }

        [Test]
        public void LoadByMail_lädt_Benutzer() {
            Repository.FindByEmail("barney@bro.com").Name.Should().Be("Barney Stinson");
        }

        [Test]
        public void LoadByMail_gibt_null_bei_unbekanntem_Benutzer() {
            Repository.FindByEmail("rag@doll.com").Should().BeNull();
        }

    }
}
