using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PropertiesMicroService.ViewModels
{
    public class viNewPropertyTag
    {
        [Required]
        public int PropertyId { get; set; }
        [Required]
        public int Tagid { get; set; }
    }
}
