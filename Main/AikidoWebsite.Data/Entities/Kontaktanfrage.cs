using System;
using System.Collections.Generic;
using AikidoWebsite.Common;
using AikidoWebsite.Data.Extensions;
using Newtonsoft.Json;

namespace AikidoWebsite.Data.Entities
{

    public class Kontaktanfrage : IEntity
    {
        public string Id { get; set; }
        public string EMailAdresse { get; set; }
        public DateTime ErstelltAm { get; set; }
        public string Name { get; set; }
        public string Nachricht { get; set; }
       
}
