using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class CompanyConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                .ToTable("Companies")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Company>()
                .Property(m => m.ID)
                .HasColumnName("CompanyID");

            modelBuilder.Entity<Company>()
                .Property(m => m.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<Company>()
                .Property(m => m.LegalEntity)
                .HasMaxLength(50);

            modelBuilder.Entity<Company>()
                .Property(m => m.COID)
                .HasMaxLength(20);

            modelBuilder.Entity<Company>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Company>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Company>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Company>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        } // Configure
    }
}