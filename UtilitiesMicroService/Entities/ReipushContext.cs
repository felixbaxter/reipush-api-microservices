using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace UtilitiesMicroService.Entities
{
    public class ReipushContext : DbContext
    {
        public ReipushContext (DbContextOptions<ReipushContext> options)
            : base(options)
        {
        }

        public virtual DbSet<UtilitiesMicroService.Entities.Countries> Countries { get; set; }
        public virtual DbSet<UtilitiesMicroService.Entities.StateProvince> StateProvinces { get; set; }
    }
}
