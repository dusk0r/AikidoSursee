using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.Models {

    public class NewAccountModel {

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "E-Mail Adresse")]
        public string EMail { get; set; }

        [Required]
        [Display(Name = "Passwort")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
