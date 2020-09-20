using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroService.Entities
{
    [Table("UserRefreshTokens")]
    public class UserRefreshToken
    {
        [Key]
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
