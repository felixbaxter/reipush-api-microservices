using AuthorizeNet.Api.Contracts.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentsMicroService.Entities
{
    public class AuthorizeNetResponse
    {
        public createCustomerProfileResponse CreateCustomerProfileResponse { get; set; }
        public getCustomerProfileResponse GetCustomerProfileResponse { get; set; }
        public updateCustomerProfileResponse UpdateCustomerProfileResponse { get; set; }
        public createCustomerPaymentProfileResponse CreateCustomerPaymentProfileResponse { get; set; }
        public getCustomerPaymentProfileResponse GetCustomerPaymentProfileResponse { get; set; }
        public updateCustomerPaymentProfileResponse UpdateCustomerPaymentProfileResponse { get; set; }
        public deleteCustomerPaymentProfileResponse DeleteCustomerPaymentProfileResponse { get; set; }
        public createTransactionResponse CreateTransactionResponse { get; set; }
        public messageTypeEnum ResponseType { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public string ResponseErrors { get; set; }


        public string CardBrand
        {
            get
            {
                var response = CreateCustomerPaymentProfileResponse.validationDirectResponse.Split(',');
                return response.Length >= 51 ? response[51] : string.Empty;
            }
        }

        public AuthorizeNetResponse()
        {
            ResponseText = "";
        }
    }
}
