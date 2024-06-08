using Arysoft.ARI.NF48.Api.Data.Configurations;
using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Arysoft.ARI.NF48.Api.Data
{
    public class AriContext : DbContext
    {
        //public DbSet<Application> Applications { get; set; }
        //public DbSet<Contact> Contacts { get; set; }
        //public DbSet<NaceCode> NaceCodes { get; set; }
        //public DbSet<Organization> Organizations { get; set; }
        //public DbSet<Shift> Shifts { get; set; }
        // public DbSet<Site> Sites { get; set; }
        // public DbSet<Standard> Standards { get; set; }
        //public DbSet<User> Users { get; set; }
        //public DbSet<Role> Roles { get; set; }

        public AriContext() : base("DefaultConnection") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            ApplicationConfiguration.Configure(modelBuilder);
            ContactConfiguration.Configure(modelBuilder);
            OrganizationConfiguration.Configure(modelBuilder);
            NaceCodeConfiguration.Configure(modelBuilder);
            RolesConfiguration.Configure(modelBuilder);
            ShiftConfiguration.Configure(modelBuilder);
            SiteConfiguration.Configure(modelBuilder);
            StandardConfiguration.Configure(modelBuilder);
            UserConfiguration.Configure(modelBuilder);
        }
    }
}