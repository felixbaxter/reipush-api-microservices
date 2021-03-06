﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroService.Entities
{
    [Table("Users")]
    public class User
    {
        public User()
        {
            CreatedOn = DateTime.UtcNow;
            UpdatedOn = DateTime.UtcNow;
        }

        [Key]
        public int UserId { get; set; }
        public bool? IsVerified { get; set; }
        public string Email { get; set; }
        public int UserTypeId { get; set; }
        public bool? IsActive { get; set; }
        public int? DeactivatedById { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string? PasswordSalt { get; set; }
        public string? PasswordHash { get; set; }
        public DateTime? LastAnnouncementDate { get; set; }
    }
}

