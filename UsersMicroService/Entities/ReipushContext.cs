using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace UsersMicroService.Entities
{
    public class ReipushContext : DbContext
    {
        public ReipushContext (DbContextOptions<ReipushContext> options)
            : base(options)
        {
        }

        public virtual DbSet<UsersMicroService.Entities.User> User { get; set; }
        public virtual DbSet<UsersMicroService.Entities.voUser> voUser { get; set; }
        public virtual DbSet<UsersMicroService.Entities.VoiceNote> VoiceNote { get; set; }
        public virtual DbSet<UsersMicroService.Entities.UserRefreshToken> UserRefreshTokens { get; set; }
        public virtual DbSet<UsersMicroService.Entities.Tag> Tag { get; set; }

    }
}
