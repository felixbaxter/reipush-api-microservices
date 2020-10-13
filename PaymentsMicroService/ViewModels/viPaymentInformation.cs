using AuthorizeNet.Api.Contracts.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentsMicroService.ViewModels
{
    public class viPaymentInformation
    {

        public string AuthNetProfileId { get; set; }
        public string cardNumber { get;set;}
        public int expMonth{ get;set;} 
        public int expYear{ get;set;} 
        public string ccv{ get;set;} 
        public decimal amount{ get;set;} 
        public string invoiceHeader{ get;set;} 
        public string description{ get;set;} 
        public int orderId{ get;set;} 
        public string firstname{ get;set;} 
        public string lastname{ get;set;} 
        public string addressline{ get;set;} 
        public string city{ get;set;} 
        public string state{ get;set;} 
        public string zip{ get;set;}

       // public transactionTypeEnum authOnlyOrCapture;
    }
}
