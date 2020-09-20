using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroService.Entities
{
    [Table("voUsers")]
    public class voUser
    {
        [Key]
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public Int16 UserType { get; set; }
    }
}
