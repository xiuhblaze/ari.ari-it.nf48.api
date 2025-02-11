using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class OrganizationStandardConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrganizationStandard>()
                .ToTable("OrganizationStandards")
                .HasKey(m => m.ID);

            modelBuilder.Entity<OrganizationStandard>()
                .Property(m => m.ID)
                .HasColumnName("OrganizationStandardID");

            modelBuilder.Entity<OrganizationStandard>()
                .Property(m => m.OrganizationID)
                .IsRequired();

            //modelBuilder.Entity<OrganizationStandard>()
            //    .Property(m => m.CRN)
            //    .HasMaxLength(10);

            modelBuilder.Entity<OrganizationStandard>()
                .Property(m => m.ExtraInfo)
                .HasMaxLength(1000);

            modelBuilder.Entity<OrganizationStandard>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<OrganizationStandard>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<OrganizationStandard>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<OrganizationStandard>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}