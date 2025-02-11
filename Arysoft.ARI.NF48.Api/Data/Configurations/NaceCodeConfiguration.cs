using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class NaceCodeConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NaceCode>()
                .ToTable("NaceCodes")
                .HasKey(m => m.ID);

            modelBuilder.Entity<NaceCode>()
                .Property(m => m.ID)
                .HasColumnName("NaceCodeID");

            modelBuilder.Entity<NaceCode>()
                .Property(m => m.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<NaceCode>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<NaceCode>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<NaceCode>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<NaceCode>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        } // Configure
    }
}