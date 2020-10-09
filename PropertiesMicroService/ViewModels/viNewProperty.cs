using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PropertiesMicroService.ViewModels
{
    public class viNewProperty
    {
        [Required]
        public int AttomId { get; set; }
        [Required]
        public int UserId { get; set; }

    }
}
