using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class AuditAuditorConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditAuditor>()
                .ToTable("AuditAuditors")
                .HasKey(m => m.ID);

            modelBuilder.Entity<AuditAuditor>()
                .Property(m => m.ID)
                .HasColumnName("AuditAuditorID");

            modelBuilder.Entity<AuditAuditor>()
                .Property(m => m.AuditID)
                .IsRequired();

            modelBuilder.Entity<AuditAuditor>()
                .Property(m => m.Comments)
                .HasMaxLength(500);

            modelBuilder.Entity<AuditAuditor>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<AuditAuditor>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<AuditAuditor>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<AuditAuditor>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}