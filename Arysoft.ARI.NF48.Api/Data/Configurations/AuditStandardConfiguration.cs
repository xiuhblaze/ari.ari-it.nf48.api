using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class AuditStandardConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditStandard>()
                .ToTable("AuditStandards")
                .HasKey(m => m.ID);

            modelBuilder.Entity<AuditStandard>()
                .Property(m => m.ID)
                .HasColumnName("AuditStandardID");

            modelBuilder.Entity<AuditStandard>()
                .Property(m => m.AuditID)
                .IsRequired();

            modelBuilder.Entity<AuditStandard>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<AuditStandard>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<AuditStandard>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<AuditStandard>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}