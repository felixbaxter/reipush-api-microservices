﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentsMicroService.ViewModels
{
    public class viEmail
    {
        [Required]
        public string Email { get; set; }
  
    }
}

