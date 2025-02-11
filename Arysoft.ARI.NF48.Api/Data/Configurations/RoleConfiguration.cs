using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class RoleConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .ToTable("Roles")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Role>()
                .Property(m => m.ID)
                .HasColumnName("RoleID");

            modelBuilder.Entity<Role>()
                .Property(m => m.Name)
                .HasMaxLength(50);

            modelBuilder.Entity<Role>()
                .Property(m => m.Description)
                .HasMaxLength(250);

            modelBuilder.Entity<Role>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50);
        } // Configure
    }
}