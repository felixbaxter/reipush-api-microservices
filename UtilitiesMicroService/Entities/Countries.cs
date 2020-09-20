using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UtilitiesMicroService.Entities
{
    [Table("Countries")]
    public class Countries
    {
        [Key]
        public int CountryId { get; set; }
        public string Name { get; set; }

        public bool AllowsRegistration { get; set; }

        public bool AllowsBilling { get; set; }

        public bool AllowsShipping { get; set; }

        public string TwoLetterISOCode { get; set; }
        public string ThreeLetterISOCode { get; set; }

        public int NumericISOCode { get; set; }

        public bool SubjectToVAT { get; set; }

        public bool Published { get; set; }

        public int DisplayOrder { get; set; }




    }
}
