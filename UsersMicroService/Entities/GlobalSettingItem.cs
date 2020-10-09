using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroService.Entities
{
    [Table("GlobalSettings")]

    public class GlobalSettingItem
    {
        [Key]
        public int GlobalSettingId { get; set; }
        public string Name { get; set; }
        public string value { get; set; }
        public string Description { get; set; }
    }
}