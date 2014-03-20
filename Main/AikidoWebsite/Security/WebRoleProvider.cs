using AikidoWebsite.Common;
using AikidoWebsite.Data.Entities;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace AikidoWebsite.Web.Security {

    public class WebRoleProvider : RoleProvider {

        [Inject]
        public IDocumentStore DocumentStore { get; set; }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames) {
            throw new NotImplementedException();
        }

        public override string ApplicationName {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName) {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch) {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles() {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username) {
            return GetGroupsForBenutzer(username).ToArray();
        }

        public override string[] GetUsersInRole(string roleName) {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName) {
            return GetGroupsForBenutzer(username).Contains(roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName) {
            throw new NotImplementedException();
        }

        private IList<string> GetGroupsForBenutzer(string username) {
            var groups = new List<string>();

            if (String.IsNullOrEmpty(username)) {
                return groups;
            }

            // Todo: config
            using (var session = DocumentStore.OpenSession()) {
                Benutzer benutzer = session.Query<Benutzer>().Where(a => a.EMail.Equals(username)).FirstOrDefault();

                if (benutzer != null) {
                    groups.AddRange(benutzer.Gruppen.Select(g => g.ToString()));
                }
            }

            return groups;
        }
    }
}