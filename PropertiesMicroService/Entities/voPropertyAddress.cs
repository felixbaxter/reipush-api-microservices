using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PropertiesMicroService.Entities
{
    [Table("voPropertyAddress")]
    public class voPropertyAddress
    {
        [Key]
        public Int32 AttomId { get; set; }
        public string PropertyAddressFull { get; set; }
        public string PropertyAddressCity { get; set; }
        public string PropertyAddressState { get; set; }
        public string PropertyAddressZip { get; set; }
        public string PropertyAddressZip4 { get; set; }
        public string SitusCounty { get; set; }
        public string ParcelNumberFormatted { get; set; }
        public string PartyOwner1NameFull { get; set; }
   
    }
}
