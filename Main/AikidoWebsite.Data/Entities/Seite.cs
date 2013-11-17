﻿using AikidoWebsite.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wiki;

namespace AikidoWebsite.Data.Entities {
    public class Seite : IEntity {
        private static readonly CreoleParser CreoleParser = new CreoleParser();

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime ErstellungsDatum { get; set; }
        public string Autor { get; set; }
        public int Revision { get; set; }
        public ISet<Seite> AlteRevisionen { get; set; }
        public string WikiCreole { get; set; }

        [JsonIgnore]
        public string Html { get { return CreoleParser.ToHTML(WikiCreole ?? ""); } }

        public Seite() {
            this.AlteRevisionen = new HashSet<Seite>();
        }

        //############################################################################
        #region Object Overrides

        public override string ToString() {
            return CreateString.From(this).UseProperties().ToString();
        }

        public override bool Equals(object obj) {
            if (Object.ReferenceEquals(this, obj)) {
                return true;
            }

            var other = obj as Seite;

            if (other == null) {
                return false;
            }

            return (Id == null) ? false : Id.Equals(other.Id) && Revision.Equals(other.Revision);
        }

        public override int GetHashCode() {
            return (Id == null) ? base.GetHashCode() : HashCode.FromObjects(Id, Revision);
        }

        #endregion
        //############################################################################
    }
}