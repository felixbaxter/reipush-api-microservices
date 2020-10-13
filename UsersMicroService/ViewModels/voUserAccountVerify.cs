using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroService.ViewModels
{
    public class voUserAccountVerify
    {
        public string Email { get; set; }
        public int UserId { get; set; }
        public bool? IsVerified { get; set; }
        public bool? IsActive { get; set; }
        public string AuthNetProfileId { get; set; }

    }
}