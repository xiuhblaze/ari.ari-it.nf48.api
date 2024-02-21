using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class UserConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(m => m.ID);

            modelBuilder.Entity<User>()
                .Property(m => m.ID)
                .HasColumnName("UserID");

            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles).WithMany(r => r.Users)
                .Map(e => e.MapLeftKey("UserID")
                    .MapRightKey("RoleID")
                    .ToTable("RolesUsers")
                );
        } // Configure
    }
}