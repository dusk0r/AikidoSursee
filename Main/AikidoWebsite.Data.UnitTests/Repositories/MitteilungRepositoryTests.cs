using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.Repositories;
using AikidoWebsite.Data.Security;
using NUnit.Framework;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using AikidoWebsite.Common;

namespace AikidoWebsite.Data.UnitTests.Repositories {
    
    [TestFixture]
    public class MitteilungRepositoryTests : RepositoryTestBase<MitteilungRepository> {
        //private IUserIdentity UserIdentity { get; set; }
        //private IClock Clock { get; set; }

        protected override void Prepare() {
            //UserIdentity = Mock<IUserIdentity>();
            //Clock = Mock<IClock>();
        }

        protected override void CreateData(IDocumentSession session) {
            var post = new Mitteilung {
                Titel = "Post 1",
                ErstelltAm = new DateTime(2012, 12, 12, 19, 58, 00),
                Text = "Neuster Post"
            };
            session.Store(post);

            var post2 = new Mitteilung {
                Titel = "Post 2",
                ErstelltAm = new DateTime(2012, 12, 11, 14, 16, 30),
                Text = "Erster Post"
            };
            session.Store(post2);

            session.SaveChanges();
        }

        [Test]
        public void GetLatestMitteilungen_liefert_neuste_Posts() {
            var posts = Repository.GetLatestMitteilungen().ToList();

            posts.Should().HaveCount(2);
            posts[0].Text.Should().Be("Neuster Post");
            posts[1].Text.Should().Be("Erster Post");
        }
    }
}
