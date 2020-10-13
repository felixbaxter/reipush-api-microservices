using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizeNet.Api.Contracts.V1;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PaymentsMicroService.Entities;
using PaymentsMicroService.Services;
using PaymentsMicroService.ViewModels;

namespace PaymentsMicroService.Controllers
{


    [Route("payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ReipushContext _context;
        private readonly IConfiguration _config;
        private static readonly ILog log = LogManager.GetLogger(typeof(PaymentsController));

        public PaymentsController(ReipushContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("createAuthorizeNetProfile")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult createAuthNetProfile(viEmail x)
        {
            PaymentService _PaymentService = new PaymentService(_context);

            string AuthNetPofileId = string.Empty;
            try
            {
                var payresponse =  _PaymentService.CIM_CreateCustomerProfile(x.Email,"ReiPush Mobile User");


                if (payresponse == null)
                {
                    return BadRequest(payresponse.CreateTransactionResponse.ToString());
                }

                AuthNetPofileId = payresponse.CreateCustomerProfileResponse.customerProfileId;
            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok(AuthNetPofileId);
        }

        [HttpPost("validatePaymentSource")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult validatePaymentSource(viPaymentInformation x)
        {
            PaymentService _PaymentService = new PaymentService(_context);

            string AuthNetPaymentProfileId = string.Empty;
            try
            {
               // Charge customer .01 to validate
               //
                var payresponse = _PaymentService.AIM_ChargeCreditCard(x.cardNumber, x.expMonth, x.expYear, x.ccv, x.amount, x.invoiceHeader, 
                                                                       x.description, x.orderId, x.firstname, x.lastname, x.addressline, x.city, 
                                                                       x.state, x.zip, transactionTypeEnum.authOnlyTransaction);

                if (payresponse.ResponseType == messageTypeEnum.Error) {
                    return BadRequest(payresponse.ResponseText.ToString() + " - " +  payresponse.ResponseErrors.ToString());
                }

                // Reverse .01 Charge
                //

                var voidresponse = _PaymentService.AIM_CIM_VoidCharge(payresponse.CreateTransactionResponse.transactionResponse.transId);

                if (voidresponse.ResponseType == messageTypeEnum.Error){
                    return BadRequest(voidresponse.ResponseText.ToString() + " - " + voidresponse.ResponseErrors.ToString());
                }


                // Create Payment Profile for this card.

                PaymentMethod pm = new PaymentMethod();
                pm.CardCode = x.ccv;
                pm.CardExpMonth = x.expMonth;
                pm.CardExpYear = x.expYear;
                pm.CardNumber = x.cardNumber;
                pm.FirstName = x.firstname;
                pm.LastName = x.lastname;
                pm.StreetAddress = x.addressline;
                pm.City = x.city;
                pm.State = x.state;
                pm.Zip = x.zip;


                var paymentprofileresponse = _PaymentService.CIM_CreateCustomerPaymentProfile(x.AuthNetProfileId, pm, false);

                if (paymentprofileresponse.ResponseType == messageTypeEnum.Error){
                    return BadRequest(paymentprofileresponse.ResponseText.ToString() + " - " + paymentprofileresponse.ResponseErrors.ToString());
                }

                // Return all needed information needed to send back to applications.

                AuthNetPaymentProfileId = paymentprofileresponse.CreateCustomerPaymentProfileResponse.customerPaymentProfileId;
            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok(AuthNetPaymentProfileId);
        }
    }

}
