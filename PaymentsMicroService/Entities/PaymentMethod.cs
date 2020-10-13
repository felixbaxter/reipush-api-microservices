using System.ComponentModel.DataAnnotations;

namespace PaymentsMicroService.Entities
{
    public class PaymentMethod
    {

        public PaymentMethod()
        {
            Updating = false;
            AuthNetPaymentProfileId = "";
        }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string StreetAddress { get; set; }

        [Required]
        public string City { get; set; }

        public string State { get; set; }

        [Required]
        public string Zip { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public string CardType { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public int CardExpMonth { get; set; }

        [Required]
        public int CardExpYear { get; set; }

        [Required]
        public string CardCode { get; set; }

        public bool Updating { get; set; }
        public int PaymentProfileId { get; set; }
        public string AuthNetPaymentProfileId { get; set; }
        public bool IsDefault { get; set; }
        public bool AllowPurchases { get; set; }

    }
}