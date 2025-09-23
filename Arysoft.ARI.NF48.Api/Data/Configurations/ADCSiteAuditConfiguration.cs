using Arysoft.ARI.NF48.Api.Models;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class ADCSiteAuditConfiguration
    {
        public static void Configure(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ADCSiteAudit>()
                .ToTable("ADCSiteAudits")
                .HasKey(m => m.ID);

            modelBuilder.Entity<ADCSiteAudit>()
                .Property(m => m.ID)
                .HasColumnName("ADCSiteAuditID");

            modelBuilder.Entity<ADCSiteAudit>()
                .Property(m => m.ADCSiteID)
                .IsRequired();

            modelBuilder.Entity<ADCSiteAudit>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<ADCSiteAudit>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<ADCSiteAudit>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<ADCSiteAudit>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}