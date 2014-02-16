﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AikidoWebsite.Web.Models {

    public class StoredDateiModel {
        public int TotalCount { get; set; }
        public int Start { get; set; }
        public int Count { get; set; }
        public IEnumerable<StoredDateiEintragModel> Dateien { get; set; }

        public StoredDateiModel() {
            this.Dateien = new List<StoredDateiEintragModel>();
        }
    }

    public class StoredDateiEintragModel {
        public string Id { get; set; }
        public string AttachmentId { get; set; }
        public DateTime ErstelltAm { get; set; }
        public string MimeType { get; set; }
        public string Name { get; set; }
        public string Beschreibung { get; set; }
        public int Bytes { get; set; }
    }

    public class FileUploadModel {
        public string Beschreibung { get; set; }
        public HttpPostedFileBase File { get; set; }
    }


}