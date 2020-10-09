using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PropertiesMicroService.Entities
{
    [Table("PropertyPhotos")]

    public class PropertyPhoto
    {
        public PropertyPhoto()
        {
            CreatedOn = DateTime.UtcNow;
            UpdatedOn = DateTime.UtcNow;
        }

        [Key]
        public int PropertyPhotoId { get; set; }
        public int PropertyId { get; set; }
        public string FileName { get; set; }
        public string LocalLocation { get; set; }
        public string RemoteLocation { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
