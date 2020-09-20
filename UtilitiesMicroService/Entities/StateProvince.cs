using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UtilitiesMicroService.Entities
{
    [Table("StateProvinces")]
    public class StateProvince
    {
        [Key]
        public int StateProvinceId { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }

        public int DisplayOrder { get; set; }
    }
}
