using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class ProposalAuditConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProposalAudit>()
                .ToTable("ProposalAudits")
                .HasKey(m => m.ID);

            modelBuilder.Entity<ProposalAudit>()
                .Property(m => m.ID)
                .HasColumnName("ProposalAuditID");

            modelBuilder.Entity<ProposalAudit>()
                .Property(m => m.ProposalID)
                .IsRequired();

            modelBuilder.Entity<ProposalAudit>()
                .Property(m => m.TotalAuditDays)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ProposalAudit>()
                .Property(m => m.CertificateIssue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProposalAudit>()
                .Property(m => m.TotalCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProposalAudit>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<ProposalAudit>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<ProposalAudit>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<ProposalAudit>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}