using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Service.Services {

    public interface ITestDataService {

        void CreateTestUser();

        void CreateTestPosts();

    }

    public class TestDataService : ITestDataService, IService {

        [Inject]
        public IUserManagementService UserManagementService { get; set; }

        [Inject]
        public IMitteilungenService MitteilungenService { get; set; }

        public void CreateTestUser() {
            var benutzer = new Benutzer("Christoph Enzmann", "chris@test.com");
            benutzer.SetAdmin(true);

            UserManagementService.CreateBenutzer(benutzer, "AAaa1234");

        }

        public void CreateTestPosts() {
            MitteilungenService.CreateMitteilung("Titel 1", "Text 123 abc def xzy", Data.ValueObjects.Publikum.Extern);
            MitteilungenService.CreateMitteilung("Titel 2", "fsdfsdg fg fgsf dfsdf sdfsd fsdf ", Data.ValueObjects.Publikum.Sursee);
        }

        public void CreateTestEvents() {

        }

    }
}
