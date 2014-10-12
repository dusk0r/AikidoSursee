﻿using AikidoWebsite.Common;
using Raven.Imports.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wiki;

namespace AikidoWebsite.Data.Entities {
    
    public class Hinweis : IEntity {
        private static readonly CreoleParser CreoleParser = new CreoleParser();

        public string Id { get; set; }
        public string Text { get; set; }
        public DateTime Enddatum { get; set; }
        
        [JsonIgnore]
        public string Html { get { return CreoleParser.ToHTML(Text ?? ""); } }

        //############################################################################
        #region Object Overrides

        public override string ToString() {
            return CreateString.From(this).UseProperties().ToString();
        }

        public override bool Equals(object obj) {
            if (Object.ReferenceEquals(this, obj)) {
                return true;
            }

            var other = obj as Hinweis;

            return (other == null) ? false : Id.Equals(other.Id);
        }

        public override int GetHashCode() {
            return (Id == null) ? base.GetHashCode() : Id.GetHashCode();
        }

        #endregion
        //############################################################################
    }
}