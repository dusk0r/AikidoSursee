using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Common {

    public interface IClock {
        DateTime Now { get; }
    }

    public class Clock : IClock {

        public DateTime Now {
            get { return DateTime.Now; }
        }
    }
}
