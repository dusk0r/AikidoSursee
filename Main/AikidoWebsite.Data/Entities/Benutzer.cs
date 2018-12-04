using System;
using System.Collections.Generic;
using AikidoWebsite.Common;
using AikidoWebsite.Data.ValueObjects;

namespace AikidoWebsite.Data.Entities
{

    public class Benutzer : IEntity {

        public Benutzer() {
            Gruppen = new HashSet<string>();
        }

        public Benutzer(string name, string email) : this() {
            this.Name = name;
            this.EMail = email;
            this.Gruppen.Add("benutzer");
            this.IstAktiv = true;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string PasswortHash { get; set; }
        public ISet<string> Gruppen { get; set; }
        public bool IstAktiv { get; set; }

        public string GoogleLogin { get; set; }
        public string TwitterLogin { get; set; }

        //############################################################################
        #region Object Overrides

        public override string ToString() {
            return CreateString.From(this).UseProperties().ToString();
        }

        public override bool Equals(object obj) {
            if (Object.ReferenceEquals(this, obj)) {
                return true;
            }

            return (!(obj is Benutzer other)) ? false : Id.Equals(other.Id);
        }

        public override int GetHashCode() {
            return (Id == null) ? base.GetHashCode() : Id.GetHashCode();
        }

        #endregion
        //############################################################################

        public void SetAdmin(bool isAdmin) {
            if (isAdmin) {
                Gruppen.Add("admin");
            } else {
                Gruppen.Remove("admin");
            }
        }

        public void SetActive(bool isActive) {
            IstAktiv = isActive;
        }

        /// <summary>
        /// Setzt ein neues Passwort
        /// </summary>
        /// <param name="password"></param>
        public void SetPassword(string password) {
            PasswortHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Überprüft das angegebene Passwort mit dem gespeicherten Hash
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckPassword(string password) {
            if (password == null) {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, PasswortHash);
        }

    }
}
