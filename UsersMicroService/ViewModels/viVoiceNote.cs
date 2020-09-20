using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroService.ViewModels
{
    public class viVoiceNote
    {
        [Required]
        public int userid { get; set; }
        public IFormFile UserForm { get; set; }
  
    }
}

