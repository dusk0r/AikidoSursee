using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AikidoWebsite.Web.Models {
    public class PhotoSetModel {

        /// <summary>
        /// URL des Vorschaubildes
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Beschreibung der Gallerie
        /// </summary>
        public string Beschreibung { get; set; }

        /// <summary>
        /// Titel der Gallerie
        /// </summary>
        public string Titel { get; set; }

        /// <summary>
        /// Id der Gallerie
        /// </summary>
        public string PhotosetId { get; set; }

        public DateTime CreationDate { get; set; }

        public string Link { get; set; }
    }

    public class BildModel {

        public string Titel { get; set; }

        public string Beschreibung { get; set; }

        public string ImageURL { get; set; }

        public string Link { get; set; }
    }
}