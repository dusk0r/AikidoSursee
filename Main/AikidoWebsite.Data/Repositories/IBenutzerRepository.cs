using AikidoWebsite.Data.Models;
using AikidoWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.Repositories {

    public interface IBenutzerRepository : IRepository<Benutzer> {

        Benutzer FindByEmail(string email);

    }
}
