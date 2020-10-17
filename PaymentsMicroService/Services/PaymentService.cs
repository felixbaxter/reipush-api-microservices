using log4net;
using PaymentsMicroService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizeNet;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;
using System.Configuration;

namespace PaymentsMicroService.Services
{
    public class PaymentService
    {

        private readonly ReipushContext _reipushcontext;
        private static readonly ILog log = LogManager.GetLogger(typeof(PaymentService));
        private string ApiLogin;
        private string TransactionKey;
        private bool TestMode;

        public PaymentService(ReipushContext context)
        {
            _reipushcontext = context;
            ApiLogin = GetValueByName("Authorize.Net.APILogin");
            TransactionKey = GetValueByName("Authorize.Net.APITrasactionKey");
            TestMode = Convert.ToBoolean(GetValueByName("Authorize.Net.TestMode"));
            //    //TestMode = true;
            //ApiLogin = "5KP3u95bQpv";
            //TransactionKey = "346HZ32z3fP4hTG2";
        }



        public AuthorizeNetResponse AIM_ChargeCreditCard(string cardNumber, int expMonth, int expYear, string ccv, decimal amount, string invoiceHeader, string description, int orderId, string firstname, string lastname, string addressline, string city, string state, string zip, transactionTypeEnum authOnlyOrCapture)
        {

                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = (TestMode) ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
                {
                    name = ApiLogin,
                    ItemElementName = ItemChoiceType.transactionKey,
                    Item = TransactionKey,
                };
                
                var creditCard = new creditCardType
                {
                    cardNumber = cardNumber,
                    expirationDate = expMonth.ToString().PadLeft(2, '0') + expYear.ToString(),
                    cardCode = ccv
                };

                var custAddress = new customerAddressType();
                if (firstname != "" && lastname != "")
                {
                    custAddress.firstName = firstname;
                    custAddress.lastName = lastname;
                    custAddress.address = addressline;
                    custAddress.city = city;
                    custAddress.state = state;
                    custAddress.zip = zip;
                }

                var addon = "";
                if (orderId > 0)
                {
                    addon = addon + "-" + orderId.ToString();
                }

                var orderInfo = new orderType
                {
                    invoiceNumber = invoiceHeader + addon,
                    description = description
                };

                //standard api call to retrieve response
                var paymentType = new paymentType { Item = creditCard };

                var transactionRequest = new transactionRequestType
                {
                    transactionType = authOnlyOrCapture.ToString(),
                    amount = amount,
                    payment = paymentType,
                    order = orderInfo
                };

                if (custAddress.firstName != "")
                {
                    transactionRequest.billTo = custAddress;
                }

                var request = new createTransactionRequest
                {
                    transactionRequest = transactionRequest,
                };

                // instantiate the contoller that will call the service
                var controller = new createTransactionController(request);
                controller.Execute();

                //Initiate our custom class to return the response
                var response = new AuthorizeNetResponse() { CreateTransactionResponse = controller.GetApiResponse() };
                response.ResponseType = response.CreateTransactionResponse.messages.resultCode;
                response.ResponseCode = response.CreateTransactionResponse.messages.message[0].code;
                response.ResponseText = response.CreateTransactionResponse.messages.message[0].text;
                    if (response.CreateTransactionResponse.transactionResponse != null && response.CreateTransactionResponse.transactionResponse.errors != null)
                    {
                        if (response.CreateTransactionResponse.transactionResponse.errors.Length > 0)
                        {
                            var s = "";
                            foreach (var e in response.CreateTransactionResponse.transactionResponse.errors)
                            {
                                s += e.errorText + "|";
                            }
                            if (s.EndsWith("|"))
                            {
                                s = s.Remove(s.Length - 1, 1);
                            }
                            response.ResponseErrors = s;
                        }
                    }


            return response;
        }

        public AuthorizeNetResponse AIM_CIM_VoidCharge(string transactionId)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = (TestMode) ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLogin,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = TransactionKey,
            };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.voidTransaction.ToString(),
                refTransId = transactionId
            };

            var request = new createTransactionRequest
            {
                transactionRequest = transactionRequest,
            };

            // instantiate the contoller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            //Initiate our custom class to return the response
            var response = new AuthorizeNetResponse() { CreateTransactionResponse = controller.GetApiResponse() };
            response.ResponseType = response.CreateTransactionResponse.messages.resultCode;
            response.ResponseCode = response.CreateTransactionResponse.messages.message[0].code;
            response.ResponseText = response.CreateTransactionResponse.messages.message[0].text;
            if (response.CreateTransactionResponse.transactionResponse != null && response.CreateTransactionResponse.transactionResponse.errors != null)
            {
                if (response.CreateTransactionResponse.transactionResponse.errors.Length > 0)
                {
                    var s = "";
                    foreach (var e in response.CreateTransactionResponse.transactionResponse.errors)
                    {
                        s += e.errorText + "|";
                    }
                    if (s.EndsWith("|"))
                    {
                        s = s.Remove(s.Length - 1, 1);
                    }
                    response.ResponseErrors = s;
                }
            }
            return response;
        }

        public AuthorizeNetResponse AIM_CIM_CapturePreviouslyAuthorizedCharge(string transactionId, decimal amountToCapture)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = (TestMode) ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLogin,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = TransactionKey,
            };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.priorAuthCaptureTransaction.ToString(),
                amount = amountToCapture,
                refTransId = transactionId
            };

            var request = new createTransactionRequest
            {
                transactionRequest = transactionRequest,
            };

            // instantiate the contoller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            //Initiate our custom class to return the response
            var response = new AuthorizeNetResponse() { CreateTransactionResponse = controller.GetApiResponse() };
            response.ResponseType = response.CreateTransactionResponse.messages.resultCode;
            response.ResponseCode = response.CreateTransactionResponse.messages.message[0].code;
            response.ResponseText = response.CreateTransactionResponse.messages.message[0].text;
            if (response.CreateTransactionResponse.transactionResponse != null && response.CreateTransactionResponse.transactionResponse.errors != null)
            {
                if (response.CreateTransactionResponse.transactionResponse.errors.Length > 0)
                {
                    var s = "";
                    foreach (var e in response.CreateTransactionResponse.transactionResponse.errors)
                    {
                        s += e.errorText + "|";
                    }
                    if (s.EndsWith("|"))
                    {
                        s = s.Remove(s.Length - 1, 1);
                    }
                    response.ResponseErrors = s;
                }
            }
            return response;
        }

        public AuthorizeNetResponse AIM_CIM_RefundCharge(string cardNumber, int expMonth, int expYear, decimal amount, string transactionId)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = (TestMode) ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLogin,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = TransactionKey,
            };

            var creditCard = new creditCardType
            {
                cardNumber = cardNumber,
                expirationDate = "XXXX" // Documentation says to pass XXXX expMonth.ToString().PadLeft(2, '0') + expYear.ToString(),
            };

            //standard api call to retrieve response
            var paymentType = new paymentType { Item = creditCard };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.refundTransaction.ToString(),    // refund type
                payment = paymentType,
                amount = amount,
                refTransId = transactionId
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the contoller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            //Initiate our custom class to return the response
            var response = new AuthorizeNetResponse() { CreateTransactionResponse = controller.GetApiResponse() };
            response.ResponseType = response.CreateTransactionResponse.messages.resultCode;
            response.ResponseCode = response.CreateTransactionResponse.messages.message[0].code;
            response.ResponseText = response.CreateTransactionResponse.messages.message[0].text;
            if (response.CreateTransactionResponse.transactionResponse != null && response.CreateTransactionResponse.transactionResponse.errors != null)
            {
                if (response.CreateTransactionResponse.transactionResponse.errors.Length > 0)
                {
                    var s = "";
                    foreach (var e in response.CreateTransactionResponse.transactionResponse.errors)
                    {
                        s += e.errorText + "|";
                    }
                    if (s.EndsWith("|"))
                    {
                        s = s.Remove(s.Length - 1, 1);
                    }
                    response.ResponseErrors = s;
                }
            }
            return response;
        }

        public AuthorizeNetResponse CIM_CreateCustomerProfile(string emailAddress, string description)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = (TestMode) ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLogin,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = TransactionKey,
            };

            if (string.IsNullOrEmpty(description))
            {
                description = "new customer profile";
            }

            var customerProfile = new customerProfileType();
            customerProfile.email = emailAddress;
            customerProfile.description = description;

            var request = new createCustomerProfileRequest { profile = customerProfile, validationMode = validationModeEnum.none };

            // instantiate the contoller that will call the service
            var controller = new createCustomerProfileController(request);
            controller.Execute();

            //Initiate our custom class to return the response
            var response = new AuthorizeNetResponse() { CreateCustomerProfileResponse = controller.GetApiResponse() };
            response.ResponseType = response.CreateCustomerProfileResponse.messages.resultCode;
            response.ResponseCode = response.CreateCustomerProfileResponse.messages.message[0].code;
            response.ResponseText = response.CreateCustomerProfileResponse.messages.message[0].text;
            return response;
        }

        public AuthorizeNetResponse CIM_GetCustomerProfile(string customerprofileId)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = (TestMode) ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLogin,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = TransactionKey,
            };

            var request = new getCustomerProfileRequest { customerProfileId = customerprofileId };

            // instantiate the controller that will call the service
            var controller = new getCustomerProfileController(request);
            controller.Execute();

            //Initiate our custom class to return the response
            var response = new AuthorizeNetResponse() { GetCustomerProfileResponse = controller.GetApiResponse() };
            response.ResponseType = response.GetCustomerProfileResponse.messages.resultCode;
            response.ResponseCode = response.GetCustomerProfileResponse.messages.message[0].code;
            response.ResponseText = response.GetCustomerProfileResponse.messages.message[0].text;
            return response;
        }

        public AuthorizeNetResponse CIM_UpdateCustomerProfile(int userId, string customerprofileId, string emailAddress)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = (TestMode) ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLogin,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = TransactionKey,
            };

            var profileReq = new customerProfileExType
            {
                merchantCustomerId = userId.ToString(),
                email = emailAddress,
                customerProfileId = customerprofileId
            };

            var request = new updateCustomerProfileRequest { profile = profileReq };

            // instantiate the controller that will call the service
            var controller = new updateCustomerProfileController(request);
            controller.Execute();

            //Initiate our custom class to return the response
            var response = new AuthorizeNetResponse() { UpdateCustomerProfileResponse = controller.GetApiResponse() };
            response.ResponseType = response.UpdateCustomerProfileResponse.messages.resultCode;
            response.ResponseCode = response.UpdateCustomerProfileResponse.messages.message[0].code;
            response.ResponseText = response.UpdateCustomerProfileResponse.messages.message[0].text;
            return response;
        }

        public AuthorizeNetResponse CIM_CreateCustomerPaymentProfile(string customerprofileId, PaymentMethod paymentMethod, bool testTransaction)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = (TestMode) ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLogin,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = TransactionKey,
            };

            var creditCard = new creditCardType
            {
                cardNumber = paymentMethod.CardNumber,
                expirationDate = paymentMethod.CardExpYear + "-" + paymentMethod.CardExpMonth.ToString().PadLeft(2, '0'),
                cardCode = paymentMethod.CardCode
            };

            var pmtType = new paymentType { Item = creditCard };
            var pmtProfile = new customerPaymentProfileType
            {
                payment = pmtType,
                billTo = new customerAddressType()
                {
                    firstName = paymentMethod.FirstName,
                    lastName = paymentMethod.LastName,
                    address = paymentMethod.StreetAddress,
                    city = paymentMethod.City,
                    state = paymentMethod.State,
                    zip = paymentMethod.Zip,
                    country = paymentMethod.CountryName
                }
            };

            var request = new createCustomerPaymentProfileRequest()
            {
                customerProfileId = customerprofileId,
                paymentProfile = pmtProfile,
                validationMode = testTransaction ? validationModeEnum.liveMode : validationModeEnum.none
            };

            // instantiate the controller that will call the service
            var controller = new createCustomerPaymentProfileController(request);
            controller.Execute();

            //Initiate our custom class to return the response
            var response = new AuthorizeNetResponse() { CreateCustomerPaymentProfileResponse = controller.GetApiResponse() };
            response.ResponseType = response.CreateCustomerPaymentProfileResponse.messages.resultCode;
            response.ResponseCode = response.CreateCustomerPaymentProfileResponse.messages.message[0].code;
            response.ResponseText = response.CreateCustomerPaymentProfileResponse.messages.message[0].text;
            return response;
        }

        public AuthorizeNetResponse CIM_GetCustomerPaymentProfile(string customerprofileId, string customerpaymentProfileId)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = (TestMode) ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLogin,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = TransactionKey,
            };

            var request = new getCustomerPaymentProfileRequest
            {
                customerProfileId = customerprofileId,
                customerPaymentProfileId = customerpaymentProfileId
            };

            // instantiate the controller that will call the service
            var controller = new getCustomerPaymentProfileController(request);
            controller.Execute();

            //Initiate our custom class to return the response
            var response = new AuthorizeNetResponse() { GetCustomerPaymentProfileResponse = controller.GetApiResponse() };
            response.ResponseType = response.GetCustomerPaymentProfileResponse.messages.resultCode;
            response.ResponseCode = response.GetCustomerPaymentProfileResponse.messages.message[0].code;
            response.ResponseText = response.GetCustomerPaymentProfileResponse.messages.message[0].text;
            return response;
        }

        //public AuthorizeNetResponse CIM_UpdateCustomerPaymentProfile(string customerprofileId, PaymentMethod paymentMethod)
        //{
        //    ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = (TestMode) ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;
        //    ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
        //    {
        //        name = ApiLogin,
        //        ItemElementName = ItemChoiceType.transactionKey,
        //        Item = TransactionKey,
        //    };

        //    var creditCard = new creditCardType
        //    {
        //        cardNumber = paymentMethod.CardNumber,
        //        expirationDate = paymentMethod.CardExpYear + "-" + paymentMethod.CardExpMonth.ToString().PadLeft(2, '0'),
        //        cardCode = paymentMethod.CardCode
        //    };

        //    var pmtType = new paymentType { Item = creditCard };
        //    var pmtProfile = new customerPaymentProfileExType
        //    {
        //        customerPaymentProfileId = paymentMethod.AuthNetPaymentProfileId,
        //        payment = pmtType,
        //        billTo = new customerAddressType()
        //        {
        //            firstName = paymentMethod.FirstName,
        //            lastName = paymentMethod.LastName,
        //            address = paymentMethod.StreetAddress,
        //            city = paymentMethod.City,
        //            state = paymentMethod.State,
        //            zip = paymentMethod.Zip,
        //            country = paymentMethod.CountryName
        //        }
        //    };

        //    var request = new updateCustomerPaymentProfileRequest()
        //    {
        //        customerProfileId = customerprofileId,
        //        paymentProfile = pmtProfile,
        //        validationMode = validationModeEnum.liveMode
        //    };

        //    // instantiate the controller that will call the service
        //    var controller = new updateCustomerPaymentProfileController(request);
        //    controller.Execute();

        //    //Initiate our custom class to return the response
        //    var response = new AuthorizeNetResponse() { UpdateCustomerPaymentProfileResponse = controller.GetApiResponse() };
        //    response.ResponseType = response.UpdateCustomerPaymentProfileResponse.messages.resultCode;
        //    response.ResponseCode = response.UpdateCustomerPaymentProfileResponse.messages.message[0].code;
        //    response.ResponseText = response.UpdateCustomerPaymentProfileResponse.messages.message[0].text;
        //    return response;
        //}

        public AuthorizeNetResponse CIM_DeletePaymentProfile(string customerProfileId, string paymentProfileId)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = (TestMode) ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLogin,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = TransactionKey,
            };

            var request = new deleteCustomerPaymentProfileRequest
            {
                customerProfileId = customerProfileId,
                customerPaymentProfileId = paymentProfileId
            };

            // instantiate the controller that will call the service
            var controller = new deleteCustomerPaymentProfileController(request);
            controller.Execute();

            //Initiate our custom class to return the response
            var response = new AuthorizeNetResponse() { DeleteCustomerPaymentProfileResponse = controller.GetApiResponse() };
            response.ResponseType = response.DeleteCustomerPaymentProfileResponse.messages.resultCode;
            response.ResponseCode = response.DeleteCustomerPaymentProfileResponse.messages.message[0].code;
            response.ResponseText = response.DeleteCustomerPaymentProfileResponse.messages.message[0].text;

            return response;

        }

        public AuthorizeNetResponse CIM_ChargeCustomerProfile(string customerProfileId, string paymentProfileId, string invoiceHeader, string description, int orderId, decimal amountToCharge, transactionTypeEnum authOnlyOrCapture)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = (TestMode) ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLogin,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = TransactionKey,
            };

            //create a customer payment profile
            var profileToCharge = new customerProfilePaymentType();
            profileToCharge.customerProfileId = customerProfileId;
            profileToCharge.paymentProfile = new paymentProfile { paymentProfileId = paymentProfileId };

            var orderInfo = new orderType
            {

                invoiceNumber = orderId > 0 ? invoiceHeader + "-" + orderId.ToString() : invoiceHeader,
                description = description
            };

            var transactionRequest = new transactionRequestType
            {
                transactionType = authOnlyOrCapture.ToString(),
                amount = amountToCharge,
                profile = profileToCharge,
                order = orderInfo
            };
            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the collector that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            //Initiate our custom class to return the response
            var response = new AuthorizeNetResponse() { CreateTransactionResponse = controller.GetApiResponse() };
            if (response.CreateTransactionResponse != null)
            {
                response.ResponseType = response.CreateTransactionResponse.messages.resultCode;
                response.ResponseCode = response.CreateTransactionResponse.messages.message[0].code;
                response.ResponseText = response.CreateTransactionResponse.messages.message[0].text;
                if (response.CreateTransactionResponse.transactionResponse != null && response.CreateTransactionResponse.transactionResponse.errors != null)
                {
                    if (response.CreateTransactionResponse.transactionResponse.errors.Length > 0)
                    {
                        var s = "";
                        foreach (var e in response.CreateTransactionResponse.transactionResponse.errors)
                        {
                            s += e.errorText + "|";
                        }
                        if (s.EndsWith("|"))
                        {
                            s = s.Remove(s.Length - 1, 1);
                        }
                        response.ResponseErrors = s;
                    }
                }
            }

            return response;

        }

        public static bool IsAVSVerified(AuthorizeNetResponse theResponse)
        {

            return new[] { "A", "E", "G", "P", "R", "S", "U", "W", "X", "Y", "Z" }.Contains(theResponse?.CreateTransactionResponse?.transactionResponse?.avsResultCode ?? string.Empty);

            /*      avsResultCode Address Verification Service(AVS) response code.      

                    One of the following:

                    A-- The street address matched, but the postal code did not.
                    B-- No address information was provided.
                    E-- The AVS check returned an error.
                    G-- The card was issued by a bank outside the U.S.and does not support AVS.
                    N-- Neither the street address nor postal code matched.
                    P-- AVS is not applicable for this transaction.
                    R-- Retry — AVS was unavailable or timed out.
                    S-- AVS is not supported by card issuer.
                    U-- Address information is unavailable.
                    W --The US ZIP + 4 code matches, but the street address does not.
                    X-- Both the street address and the US ZIP + 4 code matched.
                    Y-- The street address and postal code matched.
                    Z-- The postal code matched, but the street address did not.
            */
        }

        public string GetValueByName(string ivalue)
        {
            try
            {
                GlobalSettingItem rvalue = new GlobalSettingItem();

                if (ivalue==null) {
                    return null;
                }

                ivalue = _reipushcontext.GlobalSettings
                         .FirstOrDefault(u => u.Name == ivalue).value;
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            return ivalue;
        }



    }
}
