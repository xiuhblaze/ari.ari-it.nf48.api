using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class OrganizationConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {   
            modelBuilder.Entity<Organization>()
                .ToTable("Organizations")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Organization>()
                .Property(m => m.ID)
                .HasColumnName("OrganizationID");

            modelBuilder.Entity<Organization>()
                .Property(m => m.Name)
                .HasMaxLength(250);

            modelBuilder.Entity<Organization>()
                .Property(m => m.LegalEntity)
                .HasMaxLength(250);

            modelBuilder.Entity<Organization>()
                .Property(m => m.LogoFile)
                .HasMaxLength(250);

            modelBuilder.Entity<Organization>()
                .Property(m => m.Website)
                .HasMaxLength(250);

            modelBuilder.Entity<Organization>()
                .Property(m => m.Phone)
                .HasMaxLength(25);

            modelBuilder.Entity<Organization>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Organization>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50);
        } // Configure
    }
}