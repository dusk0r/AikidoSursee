using AikidoWebsite.Data.Entities;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AikidoWebsite.Data.Index {
    
    /// <summary>
    /// Index für die Zuordnung Termin -> Mitteilung
    /// </summary>
    public class MitteilungByTerminIndex : AbstractIndexCreationTask<Mitteilung> {

        public MitteilungByTerminIndex() {
            Map = mitteilungen => from m in mitteilungen
                                  from terminId in m.TerminIds
                                  select new {
                                      MitteilungId = m.Id,
                                      TerminId = terminId
                                  };
        }
    }
}
