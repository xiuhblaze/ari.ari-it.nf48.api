using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class UserConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {   
            modelBuilder.Entity<User>()
                .ToTable("Users")
                .HasKey(m => m.ID);

            modelBuilder.Entity<User>()
                .Property(m => m.ID)
                .HasColumnName("UserID");

            modelBuilder.Entity<User>()
                .Property(m => m.Username)
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(m => m.PasswordHash)
                .HasMaxLength(64);

            modelBuilder.Entity<User>()
                .Property(m => m.Email)
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(m => m.FirstName)
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(m => m.LastName)
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(m => m.UpdatedUser)
                .IsRequired()
                .HasMaxLength(50);

            // RELATIONS

            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles).WithMany(r => r.Users)
                .Map(e => e.MapLeftKey("UserID")
                    .MapRightKey("RoleID")
                    .ToTable("RolesUsers")
                );
        } // Configure
    }
}