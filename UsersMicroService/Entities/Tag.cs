using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroService.Entities
{
    [Table("Tags")]

    public class Tag
    {
        [Key]
        public int TagId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
    }
}
