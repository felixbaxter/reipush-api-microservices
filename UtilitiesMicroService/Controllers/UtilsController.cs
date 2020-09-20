using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using log4net;
using UtilitiesMicroService.Entities;
using UtilitiesMicroService.ViewModels;
using UtilitiesMicroService.Services;

namespace UtilitiesMicroService.Controllers
{
    [Route("Util")]
    [ApiController]
    public class UtilsController : ControllerBase
    {
        private readonly ReipushContext _context;
        private static readonly ILog log = LogManager.GetLogger(typeof(UtilsController));

        public UtilsController(ReipushContext context)
        {
            _context = context;
        }

        // GET: api/Util/countriesusstates
        [HttpGet("countriesusstates")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<viUsCountriesStates> GetCountriesUSStatesProv()
        {
            viUsCountriesStates result;
            try
            {
                Services.UtilService _UtilsService = new UtilService(_context);
                result = _UtilsService.GetUCountryUSStates(1);

            }catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }

            return Ok(result);
        }

        // POST: api/Util/countriesusstates
        [HttpPost("statesprovbycountryid")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<viStateProvince>> GetStatesProvByCntryId(viCountryID CntryId) 
        {
            List<viStateProvince> result;
            try
            {
                Services.UtilService _UtilsService = new UtilService(_context);
                result =  _UtilsService.GetStateProvincesByCntryId(CntryId.CountryId);

                if (result == null)
                {
                    return NotFound("No Data Found");
                }

            }catch(Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }

            return Ok(result);

        }


    }
}
