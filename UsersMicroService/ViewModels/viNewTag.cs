using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroService.ViewModels
{
    public class viNewTag
    {   [Required]
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }

    }
}
