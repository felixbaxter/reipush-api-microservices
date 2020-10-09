using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace PaymentsMicroService.Entities
{
    public class ReipushContext : DbContext
    {
        public ReipushContext (DbContextOptions<ReipushContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PaymentsMicroService.Entities.GlobalSettingItem> GlobalSettings { get; set; }
        //public virtual DbSet<PropertiesMicroService.Entities.Property>  Property { get; set; }
        //public virtual DbSet<PropertiesMicroService.Entities.PropertyTag> PropertyTag { get; set; }
        //public virtual DbSet<PropertiesMicroService.Entities.PropertyVoiceNote> PropertyVoiceNote { get; set; }
        //public virtual DbSet<PropertiesMicroService.Entities.PropertyPhoto> PropertyPhoto { get; set; }
        //public virtual DbSet<PropertiesMicroService.Entities.VoiceNote> VoiceNote { get; set; }
        //public virtual DbSet<PropertiesMicroService.Entities.Tag> Tag { get; set; }

    }
}
