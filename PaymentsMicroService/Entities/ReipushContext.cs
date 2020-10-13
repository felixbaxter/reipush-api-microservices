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

    }
}
