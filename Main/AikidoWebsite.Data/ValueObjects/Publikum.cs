using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.ValueObjects {

    public enum Publikum {
        /// <summary>
        /// Was im Dojo Sursee stattfindet
        /// </summary>
        Sursee = 1,
        /// <summary>
        /// Was ausserhalb stattfindet
        /// </summary>
        Extern = 2,
        /// <summary>
        /// Was nur Mitglieder des Dojo Sursee betrifft
        /// </summary>
        Mitglieder = 3
    }
}
