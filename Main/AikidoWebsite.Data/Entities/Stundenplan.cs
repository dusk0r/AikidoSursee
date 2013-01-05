using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.Entities {
    
    public class Stundenplan {
        public string Id { get; set; }

        public Tagesplan Montag { get; set; }
        public Tagesplan Dienstag { get; set; }
        public Tagesplan Mittwoch { get; set; }
        public Tagesplan Donnerstag { get; set; }
        public Tagesplan Freitag { get; set; }

        public Stundenplan() {
            this.Montag = new Tagesplan();
            this.Dienstag = new Tagesplan();
            this.Mittwoch = new Tagesplan();
            this.Donnerstag = new Tagesplan();
            this.Freitag = new Tagesplan();
        }
    }

    public class Tagesplan {
        public Block Morgen { get; set; }
        public Block Nachmittag { get; set; }
        public Block Abend { get; set; }

        public Tagesplan() {
            this.Morgen = new Block();
            this.Nachmittag = new Block();
            this.Abend = new Block();
        }
    }

    public class Block {
        public DateTime Startzeit { get; set; }
        public DateTime Endzeit { get; set; }
        public string Titel { get; set; }
        public string Bezeichnung { get; set; }
    }
}
