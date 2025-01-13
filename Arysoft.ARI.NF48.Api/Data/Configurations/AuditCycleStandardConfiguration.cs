using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class AuditCycleStandardConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditCycleStandard>()
                .ToTable("AuditCycleStandards")
                .HasKey(m => m.ID);

            modelBuilder.Entity<AuditCycleStandard>()
                .Property(m => m.ID)
                .HasColumnName("AuditCycleStandardID");

            modelBuilder.Entity<AuditCycleStandard>()
                .Property(m => m.AuditCycleID)
                .IsRequired();

            modelBuilder.Entity<AuditCycleStandard>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<AuditCycleStandard>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<AuditCycleStandard>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<AuditCycleStandard>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}