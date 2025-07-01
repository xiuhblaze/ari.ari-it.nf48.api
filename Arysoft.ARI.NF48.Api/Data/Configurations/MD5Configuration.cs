using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class MD5Configuration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MD5>()
                .ToTable("MD5")
                .HasKey(m => m.ID);

            modelBuilder.Entity<MD5>()
                .Property(m => m.ID)
                .HasColumnName("MD5ID");

            modelBuilder.Entity<MD5>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<MD5>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<MD5>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<MD5>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}