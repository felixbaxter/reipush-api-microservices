using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PropertiesMicroService.ViewModels
{
    public class viUpdateProperty
    {
        [Required]
        public int PropertyId { get; set; }
        public int PropertyShareDetailsid { get; set; }
        public bool Deleted { get; set; }

    }
}
