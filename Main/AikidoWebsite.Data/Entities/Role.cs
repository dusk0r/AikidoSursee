using System;
using System.Collections.Generic;
using System.Text;

namespace AikidoWebsite.Data.Entities
{
    public class Role : IEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
