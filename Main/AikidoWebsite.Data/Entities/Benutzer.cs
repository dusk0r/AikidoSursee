using AikidoWebsite.Common;
using AikidoWebsite.Data;
using AikidoWebsite.Data.Converter;
using AikidoWebsite.Data.Entities;
using AikidoWebsite.Data.ValueObjects;
using Raven.Client;
using Raven.Imports.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AikidoWebsite.Data.Entities {

    public class Benutzer : IEntity {

        public Benutzer() {
            Gruppen = new HashSet<Gruppe>();
        }

        public Benutzer(string name, string email) : this() {
            this.Name = name;
            this.EMail = email;
            this.Gruppen.Add(Gruppe.Benutzer);
            this.IstAktiv = true;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string PasswortHash { get; set; }
        [JsonConverter(typeof(GroupSetConverter))]
        public ISet<Gruppe> Gruppen { get; set; }
        public bool IstAktiv { get; set; }

        //############################################################################
        #region Object Overrides

        public override string ToString() {
            return CreateString.From(this).UseProperties().ToString();
        }

        public override bool Equals(object obj) {
            if (Object.ReferenceEquals(this, obj)) {
                return true;
            }

            var other = obj as Benutzer;

            return (other == null) ? false : Id.Equals(other.Id);
        }

        public override int GetHashCode() {
            return (Id == null) ? base.GetHashCode() : Id.GetHashCode();
        }

        #endregion
        //############################################################################

        public void SetAdmin(bool isAdmin) {
            if (isAdmin) {
                Gruppen.Add(Gruppe.Admin);
            } else {
                Gruppen.Remove(Gruppe.Admin);
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

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            uint diff = (uint)PasswortHash.Length ^ (uint)hashedPassword.Length;

            for (int i = 0; i < PasswortHash.Length && i < hashedPassword.Length; i++) {
                diff |= (uint)PasswortHash[i] ^ (uint)hashedPassword[i];
            }

            return diff == 0;
        }

    }
}
