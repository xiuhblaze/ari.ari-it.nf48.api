using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class OrganizationFamilyConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrganizationFamily>()
                .ToTable("OrganizationsFamilies")
                .HasKey(m => m.ID);

            modelBuilder.Entity<OrganizationFamily>()
                .Property(m => m.ID)
                .HasColumnName("OrganizationFamilyID");

            modelBuilder.Entity<OrganizationFamily>()
                .Property(m => m.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<OrganizationFamily>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<OrganizationFamily>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<OrganizationFamily>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<OrganizationFamily>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}