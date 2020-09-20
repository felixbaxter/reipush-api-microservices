using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PropertiesMicroService.Entities;
using PropertiesMicroService.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PropertiesMicroService.Services
{
    public class PropertyService
    {
        private readonly ReipushContext _reipushcontext;
        private static readonly ILog log = LogManager.GetLogger(typeof(PropertyService));

        public PropertyService(ReipushContext context)
        {
            _reipushcontext = context;
 
        }

        public PropertyService()
        {
        }

        public Property GetPropertyByPropId(int propid)
        {
            return _reipushcontext.Property.FirstOrDefault(p => p.PropertyId == propid);

        }
        public List<voPropertyAddress> QuickSearchAddress(viSearchAddress iaddress)
        {
            List<voPropertyAddress> rAddresses = new List<voPropertyAddress>();

            try
            {
                var vAddresses = _reipushcontext.voPropertyAddress.FromSqlRaw("SearchStreetAddress @StreetAddress, @City, @State, @Zip",
                              new SqlParameter("StreetAddress", iaddress.StreetAddress),
                              new SqlParameter("City", iaddress.City),
                              new SqlParameter("State", iaddress.State),
                              new SqlParameter("Zip", iaddress.Zip))
                             .AsEnumerable();

                rAddresses = vAddresses.ToList();
            }
            catch (Exception e)
            {

            }
            return rAddresses;
        }
        public List<Tag> GetPropertyTags(int propertyid)
        {
            List<Tag> rTags = new List<Tag>();
            try
            {
                rTags = (List<Tag>)(from p in _reipushcontext.PropertyTag
                                       join t in _reipushcontext.Tag
                                       on p.TagId equals t.TagId
                                       where p.PropertyId == propertyid
                                       select new Tag
                                       {
                                           Name = t.Name,
                                           TagId = t.TagId
                                       });

            }
            catch (Exception e)
            {
                log.Error(e);
               return rTags;
            }
            return rTags;

        }
        public List<VoiceNote> GetPropertyVoiceNotes(int propertyid)
        {
            List<VoiceNote> rVoiceNotes = new List<VoiceNote>();

            try
            {
                var rVnotes = from p in _reipushcontext.PropertyVoiceNote
                              join v in _reipushcontext.VoiceNote
                              on p.VoiceNoteId equals v.VoiceNoteId
                              where p.PropertyId == propertyid
                              select new VoiceNote
                              {
                                  CreatedOn = v.CreatedOn,
                                  FileName = v.FileName,
                                  VoiceNoteId = v.VoiceNoteId,
                                  Deleted = v.Deleted,
                                  LocalLocation = v.LocalLocation,
                                  RemoteLocation = v.RemoteLocation,
                                  UpdatedOn = v.UpdatedOn,
                                  UserId = v.UserId
                              };

                rVoiceNotes.AddRange(rVnotes);

            }
            catch (Exception e)
            {
                log.Error(e, e);
                return rVoiceNotes;
            }
            return rVoiceNotes;
        }
        public List<Property> CreateProperty(List<viNewProperty> iprop)
        {
            List<Property> rProperties = new List<Property>();
            foreach (var prop in iprop)
            {
                var vprop = _reipushcontext.Property
                            .FirstOrDefault(p => p.AttomId == prop.AttomId
                                            && p.UserId == prop.UserId);

                if (vprop == null)
                {

                    vprop = new Property()
                    {

                        AttomId = prop.AttomId,
                        UserId = prop.UserId
                    };

                    _reipushcontext.Property.Add(vprop);
                    _reipushcontext.SaveChanges();

                    rProperties.Add(vprop);


                }
            }

            return rProperties;
        }
        public List<Property> UpdateProperty(List<viUpdateProperty> iprop)
        {
            List<Property> rProperties = new List<Property>();
            foreach (var prop in iprop)
            {
                var vprop = _reipushcontext.Property
                            .Where(p => p.PropertyId == prop.PropertyId)
                            .Single();

                if (prop.PropertyShareDetailsid > 0)
                {
                    vprop.PropertyShareDetailsid = prop.PropertyShareDetailsid;
                }
                vprop.Deleted = prop.Deleted;

                _reipushcontext.SaveChanges();

                rProperties.Add(vprop);


            }
            return rProperties;
        }
        public List<PropertyTag> CreatePropertyTags(List<viNewPropertyTag> iprop)
        {
            List<PropertyTag> rProperties = new List<PropertyTag>();
            foreach (var prop in iprop)
            {
                // Ensure the tags exist on the user tags table

                var itags = _reipushcontext.Tag
                    .FirstOrDefault(i => i.TagId == prop.Tagid);

                if (itags != null)
                {

                    var vprop = _reipushcontext.PropertyTag
                                .FirstOrDefault(p => p.PropertyId == prop.PropertyId
                                                && p.TagId == prop.Tagid);

                    if (vprop == null)
                    {

                        vprop = new PropertyTag()
                        {
                            TagId = prop.Tagid,
                            PropertyId = prop.PropertyId
                        };

                        _reipushcontext.PropertyTag.Add(vprop);
                        _reipushcontext.SaveChanges();

                        rProperties.Add(vprop);


                    } 
                }
            }

            return rProperties;
        }
        public bool TogglePropertyTag(viNewPropertyTag iprop)
        {

            bool StatusChagned = false;

            try
            {
             
                // If tagid is found on the propertytag table it will be removed.  
                // If tagid is not found on the propertytag table it will be added.
                //
                    var vprop = _reipushcontext.PropertyTag
                                .FirstOrDefault(p => p.PropertyId == iprop.PropertyId
                                                && p.TagId == iprop.Tagid);

                    if (vprop == null){
                
                        vprop = new PropertyTag(){

                            TagId = iprop.Tagid,
                            PropertyId = iprop.PropertyId
                        };
                          _reipushcontext.PropertyTag.Add(vprop);
                    }
                    else {
                        _reipushcontext.PropertyTag.Remove(vprop);
                    }
                    _reipushcontext.SaveChanges();
                

                StatusChagned = true;
            }
            catch ( Exception e)
            {
                log.Error(e, e);
                StatusChagned = false;

            }
            return StatusChagned;
        }
        public async Task<bool> CreateAndStoreVoiceNotes(Property property, IFormFile iform)
        {
            // Create the VoiceNote Record
            bool isSaveSuccess = false;

            try
            {
                VoiceNote vnote = await Helper.CreateVoiceNote(property.UserId, iform);
                if (vnote != null)
                {
                    _reipushcontext.VoiceNote.Add(vnote);
                    _reipushcontext.SaveChanges();

                }

                // Tie the VoiceNote to the Property.

                var propertyvoicenote = new PropertyVoiceNote()
                {
                    PropertyId = property.PropertyId,
                    VoiceNoteId = vnote.VoiceNoteId
                };
                _reipushcontext.PropertyVoiceNote.Add(propertyvoicenote);
                _reipushcontext.SaveChanges();

                isSaveSuccess = true;

            }
            catch (Exception e)
            {
                log.Error(e, e);
                return isSaveSuccess;
            }

            return isSaveSuccess;

        }
        public List<voDeletedPropertyVoiceNote> DeletePropertyVoiceNote(List<viDeletePropertyVoiceNote> properties)
        {
            List<voDeletedPropertyVoiceNote> returnArrayObj = 
                         new List<voDeletedPropertyVoiceNote>();

            voDeletedPropertyVoiceNote returnObj;

            try
            {
                foreach(viDeletePropertyVoiceNote prop in properties)
                {
                    returnObj = new voDeletedPropertyVoiceNote();


                    // -- VoiceNote Table

                    var vnotes = _reipushcontext.VoiceNote
                                .Single(v => v.VoiceNoteId == prop.VoiceNoteId);
                    vnotes.Deleted = true;

                    // -- PropertyVoiceNote Table

                    var PropVNote = _reipushcontext.PropertyVoiceNote
                                    .Single(pv => pv.PropertyId == prop.PropertyId 
                                    && pv.VoiceNoteId == prop.VoiceNoteId);

                    _reipushcontext.Remove(PropVNote);

                    returnObj.VoiceNoteId = vnotes.VoiceNoteId;
                    returnObj.IsDeleted = true;
                    _reipushcontext.SaveChanges();

                    returnArrayObj.Add(returnObj);

                }


            }
            catch(Exception e)
            {
                log.Error(e, e);
                return null;
            }

            return returnArrayObj;
        }
        public async Task<PropertyPhoto> CreateUploadPhoto(Property iprop, IFormFile iform)
        {
            string fileName;
            PropertyPhoto pphoto = new PropertyPhoto();
            try
            {
                var extension = "." + iform.FileName.Split('.')[iform.FileName.Split('.').Length - 1];
                fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files\\Photos");

                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(pathBuilt);
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files\\Photos",
                   fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await iform.CopyToAsync(stream);
                }

                // Create the record in the VoiceNotes Database

                pphoto = new PropertyPhoto()
                {
                     PropertyId = iprop.PropertyId,
                     FileName = fileName,
                     LocalLocation = pathBuilt
                };

            }
            catch (Exception e)
            {
                log.Error(e, e);
                pphoto = null;
            }

            return pphoto;

        }
        public List<Property> GetDeletedProperty(int userid)
        {
            var delProp = (List<Property>)(from p in _reipushcontext.Property
                           where p.UserId == userid
                           && p.Deleted
                           select p);

            return delProp;
        }

        public List<Property> RestoreDeletedProperty(int userid)
        {
            var delProp = (List<Property>)(from p in _reipushcontext.Property
                                           where p.UserId == userid
                                           && p.Deleted
                                           select p);

            return delProp;
        }

    }

}
