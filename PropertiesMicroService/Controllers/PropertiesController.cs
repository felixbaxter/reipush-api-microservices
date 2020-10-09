
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PropertiesMicroService.Entities;
using PropertiesMicroService.Services;
using PropertiesMicroService.ViewModels;

namespace PropertiesMicroService.Controller
{
    [Route("property")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {

        private readonly ReipushContext _context;
        private static readonly ILog log = LogManager.GetLogger(typeof(PropertiesController));

        public PropertiesController(ReipushContext context)
        {
            _context = context;
        }

        [HttpPost("quicksearch")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<IEnumerable<voPropertyAddress>> QuickSearch(viSearchAddress iAddress)
        {
            Services.PropertyService _PropertyService = new PropertyService(_context);

            string iStreet = iAddress.StreetAddress.Trim();

            if (iStreet.Length < 4)
            {

                return BadRequest("The Address must contain 4 or more charcters");
            }

            List<voPropertyAddress> raddresses = new List<voPropertyAddress>();
            try
            {
                raddresses = _PropertyService.QuickSearchAddress(iAddress);


                if (raddresses == null)
                {
                    return NotFound("No Address Found");
                }

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok(raddresses);
        }


        [HttpPost("createproperties")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<List<Property>> CreateNew(List<viNewProperty> iprop)
        {
            Services.PropertyService _PropertyService = new PropertyService(_context);
            List<Property> propertyCreated = new List<Property>();

            try
            {
                if ((iprop == null) || (iprop.Count < 1)) {
                   return BadRequest("You must provide all required data to add a property.");
                  }


                propertyCreated = _PropertyService.CreateProperty(iprop);

                if (propertyCreated==null)
                {
                    return BadRequest("Not Created");
                }

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return propertyCreated;

        }


        [HttpPost("updateproperties")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<List<Property>> UpdateProperty(List<viUpdateProperty> iprop)
        {
            Services.PropertyService _PropertyService = new PropertyService(_context);
            List<Property> propertyUpdated = new List<Property>();

            try
            {
                if ((iprop == null) || (iprop.Count < 1))
                {
                        return BadRequest("You must provide all required data to update a property.");
                }

                propertyUpdated = _PropertyService.UpdateProperty(iprop);

                if (propertyUpdated == null)
                {
                    return BadRequest("Not Updated");
                }

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return propertyUpdated;

        }

        [HttpPost("deleteproperties")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<List<Property>> DeleteProperty(List<viUpdateProperty> iprop)
        {
            Services.PropertyService _PropertyService = new PropertyService(_context);
            List<Property> propertyUpdated = new List<Property>();

            try
            {
                if ((iprop == null) || (iprop.Count < 1))
                {
                    return BadRequest("You must provide all required data to update a property.");
                }

                propertyUpdated = _PropertyService.UpdateProperty(iprop);

                if (propertyUpdated == null)
                {
                    return BadRequest("Not Updated");
                }

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return propertyUpdated;

        }


        [HttpPost("restoreproperties")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<List<Property>> RestoreProperty(List<viUpdateProperty> iprop)
        {
            Services.PropertyService _PropertyService = new PropertyService(_context);
            List<Property> propertyUpdated = new List<Property>();

            try
            {
                if ((iprop == null) || (iprop.Count < 1))
                {
                    return BadRequest("You must provide all required data to update a property.");
                }

                propertyUpdated = _PropertyService.UpdateProperty(iprop);

                if (propertyUpdated == null)
                {
                    return BadRequest("Not Updated");
                }

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return propertyUpdated;

        }

        [HttpPost("addpropertytags")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<List<PropertyTag>> AddPropertyTags(List<viNewPropertyTag> itag)
        {
            Services.PropertyService _PropertyService = new PropertyService(_context);
            List<PropertyTag> PropTagCreated = new List<PropertyTag>();

            try
            {
                if (itag!= null)
                {
                    return BadRequest("You must provide all required data to add a tag.");
                }

                PropTagCreated = _PropertyService.CreatePropertyTags(itag);

                if (PropTagCreated == null)
                {
                    return BadRequest("Not Created");
                }

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok(PropTagCreated);
        }

        [HttpPost("togglepropertytag")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<bool> ToggledPropertyTags(viNewPropertyTag itag)
        {
            Services.PropertyService _PropertyService = new PropertyService(_context);
            bool PropTagToggled = false;

            try
            {
                if (itag != null)
                {
                    return BadRequest("You must provide all required data to add a tag.");
                }

                PropTagToggled = _PropertyService.TogglePropertyTag(itag);


            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok(PropTagToggled);
        }



        [HttpPost("addpropertyvoicenote")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> AddPropertyVoiceNotes(int propertyid, IFormFile iform)
        {
            Services.PropertyService _PropertyService = new PropertyService(_context);
            Property rvalue = new Property();
            try
            {
                if (propertyid > 1) {

                    rvalue = _PropertyService.GetPropertyByPropId(propertyid);
                    if (rvalue == null)  {
                        return BadRequest("You must provide all required data to add a voicenote.");
                    }

                }
                else  {
                    return BadRequest("You must provide all required data to add a voicenote.");
                }


                if (!Helper.IsValidFileExtention(iform.FileName, 0)){
                    return BadRequest("The file type you are trying to upload is not valid.");

                }


                bool isCreated = await _PropertyService.CreateAndStoreVoiceNotes(rvalue, iform);


                if (!isCreated)
                {
                    return BadRequest("Not Created");
                }


            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok();
        }

        [HttpPost("getallpropertyvoicenotes")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<List<VoiceNote>> ListPropertyVoiceNotes(int propertyid)
        {
            Services.PropertyService _PropertyService = new PropertyService(_context);
            List<VoiceNote> MyPropVoiceNotes = new List<VoiceNote>();

            try
            {
                if ((propertyid > 0))
                {
                    MyPropVoiceNotes = _PropertyService.GetPropertyVoiceNotes(propertyid);
                }
                else
                {
                    return BadRequest("You must provide a valid propertyid.");

                }

                if (MyPropVoiceNotes == null)
                {
                    return BadRequest("No Records Found");
                }

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok(MyPropVoiceNotes);
        }

        [HttpPost("getallpropertytags")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<List<Tag>> ListPropertyTags(int propertyid)
        {
            Services.PropertyService _PropertyService = new PropertyService(_context);
            List<Tag> MyPropTags = new List<Tag>();

            try
            {
                if ((propertyid > 0))
                {
                    MyPropTags = _PropertyService.GetPropertyTags(propertyid);
                }
                else
                {
                    return BadRequest("You must provide a valid propertyid.");

                }

                if (MyPropTags == null)
                {
                    return BadRequest("No Records Found");
                }

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok(MyPropTags);
        }

        [HttpPost("deletepropertyvoicenotes")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<List<voDeletedPropertyVoiceNote>> DeletePropertyVoiceNotes(List<viDeletePropertyVoiceNote> properties)
        {
            Services.PropertyService _PropertyService = new PropertyService(_context);
            List<voDeletedPropertyVoiceNote> MyPropVoiceNotes = new List<voDeletedPropertyVoiceNote>();

            try
            {
                if ((properties.Count > 0))
                {
                    MyPropVoiceNotes = _PropertyService.DeletePropertyVoiceNote(properties);
                }
                else
                {
                    return BadRequest("You must provide a valid propertyid.");

                }

                if (MyPropVoiceNotes == null)
                {
                    return BadRequest("No Records Found");
                }

            }
            catch (Exception e)
            {

                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok(MyPropVoiceNotes);
        }


        [HttpPost("uploadpropertyphoto")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UploadPropertyPhoto(int propertyid, IFormFile iform)
        {
            Services.PropertyService _PropertyService = new PropertyService(_context);
            Property rvalue = new Property();
            try
            {
                if (propertyid > 0)
                {

                    rvalue = _PropertyService.GetPropertyByPropId(propertyid);
                    if (rvalue == null)
                    {
                        return BadRequest("You must provide all required data to add a photo.");
                    }

                }
                else
                {
                    return BadRequest("You must provide all required data to add a photo.");
                }


                if (!Helper.IsValidFileExtention(iform.FileName, 1))
                {
                    return BadRequest("The file type you are trying to upload is not valid.");

                }


                PropertyPhoto isCreated = await _PropertyService.CreateUploadPhoto(rvalue, iform);
                

                if (isCreated==null)
                {
                    return BadRequest("Not Created");
                }


            }
            catch (Exception e)
            {

                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok();
        }

        [HttpPost("listdeletedproperties")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public List<Property> ListDeletedProperty(int userid)
        {
            Services.PropertyService _PropertyService = new PropertyService(_context);
            List<Property> rvalue = new List<Property>();
            try
            {
                if (userid > 0)
                {
                    rvalue = _PropertyService.GetDeletedProperty(userid);
                }


            }
            catch (Exception e)
            {
                log.Error(e);
                return null;
            }
            return rvalue;
        }



    }

}