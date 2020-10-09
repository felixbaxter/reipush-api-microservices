using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                    return BadRequest(payresponse.ResponseErrors.ToString());
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


    }

}
