using AikidoWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.Repositories {

    public interface IMitteilungRepository : IRepository<Mitteilung> {

        //Mitteilung CreateMitteilung(Mitteilung post);

        IEnumerable<Mitteilung> GetLatestMitteilungen(int start, int count);
    }
}
