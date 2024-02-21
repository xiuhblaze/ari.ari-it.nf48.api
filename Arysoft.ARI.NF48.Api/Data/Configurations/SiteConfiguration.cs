using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class SiteConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Site>()
                .HasKey(m => m.ID);

            modelBuilder.Entity<Site>()
                .Property(m => m.ID)
                .HasColumnName("SiteID");
        } // Configure
    }
}