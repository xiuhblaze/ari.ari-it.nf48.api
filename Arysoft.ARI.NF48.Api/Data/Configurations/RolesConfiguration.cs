using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class RolesConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .HasKey(m => m.ID);

            modelBuilder.Entity<Role>()
                .Property(m => m.ID)
                .HasColumnName("RoleID");
        } // Configure
    }
}