using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PropertiesMicroService.Entities
{
    [Table("PropertyTags")]
    public class PropertyTag
    {
        [Key]
        public int PropertyTagId { get; set; }
        public int PropertyId { get; set; }
        public int TagId { get; set; }
    }
}
