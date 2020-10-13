using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentsMicroService.Entities
{
    [Table("UserAccountPayments")]
    public class AccountPaymentProfile
    {

        public AccountPaymentProfile()
        {
            CreatedOn = DateTime.UtcNow;
            UpdatedOn = DateTime.UtcNow;
        }

        [Key]
        public int UserId { get; set; }
        public bool? IsDefault { get; set; }
        public string AuthNetPaymentProfileId { get; set; }
        public string CardType { get; set; }
        public string? Last6 { get; set; }
        public DateTime? ExpirationMonth { get; set; }
        public DateTime? ExpirationYear { get; set; }
        public bool? UseForSubscription { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
