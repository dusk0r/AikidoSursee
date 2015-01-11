using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace AikidoWebsite.Web.Security {
    public class WebMembershipUser : MembershipUser {
        private WebMembershipProvider Provider { get; set; }
        private string InternalUserName { get; set; }

        public override string UserName {
            get {
                return InternalUserName;
            }
        }

        public WebMembershipUser(WebMembershipProvider provider, string username) {
            this.Provider = provider;
            this.InternalUserName = username;
        }

        public override bool ChangePassword(string oldPassword, string newPassword) {
            return Provider.ChangePassword(InternalUserName, oldPassword, newPassword);
        }
    }
}