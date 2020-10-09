using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PropertiesMicroService.Entities
{
    [Table("Properties")]

    public class Property
    {
        public Property()
        {
            CreatedOn = DateTime.UtcNow;
            UpdatedOn = DateTime.UtcNow;
        }

        [Key]
        public int PropertyId { get; set; }
        public int AttomId { get; set; }
        public int UserId { get; set; }
        public int PropertyShareDetailsid { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
