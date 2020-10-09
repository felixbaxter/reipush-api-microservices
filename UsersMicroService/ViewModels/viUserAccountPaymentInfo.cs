using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroService.ViewModels
{
    public class viUserAccountPaymentInfo
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string CardHolderName { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string ExpirationDate { get; set; }
        [Required]
        public string CVC { get; set; }
        [Required]
        public string BillingAddress1 { get; set; }
        [Required]
        public string BillingAddress2 { get; set; }

    }
}