using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class ProposalConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Proposal>()
                .ToTable("Proposals")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Proposal>()
                .Property(m => m.ID)
                .HasColumnName("ProposalID");

            modelBuilder.Entity<Proposal>()
                .Property(m => m.AuditCycleID)
                .IsRequired();

            modelBuilder.Entity<Proposal>()
                .Property(m => m.SignerName)
                .HasMaxLength(150);

            modelBuilder.Entity<Proposal>()
                .Property(m => m.SignerPosition)
                .HasMaxLength(100);

            modelBuilder.Entity<Proposal>()
                .Property(m => m.SignedFilename)
                .HasMaxLength(250);

            modelBuilder.Entity<Proposal>()
                .Property(m => m.ExchangeRate)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Proposal>()
                .Property(m => m.TaxRate)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Proposal>()
                .Property(m => m.ExtraInfo)
                .HasMaxLength(1000);

            modelBuilder.Entity<Proposal>()
                .Property(m => m.CreatedBy)
                .HasMaxLength(50);

            modelBuilder.Entity<Proposal>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Proposal>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Proposal>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Proposal>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();

            // RELATIONS

            modelBuilder.Entity<Proposal>()
                .HasMany(m => m.ProposalAudits)
                .WithRequired(m => m.Proposal)
                .HasForeignKey(m => m.ProposalID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Proposal>()
                .HasMany(m => m.Notes)
                .WithOptional()
                .HasForeignKey(m => m.OwnerID)
                .WillCascadeOnDelete(true);

            // NOT MAPPED

            modelBuilder.Entity<Proposal>()
                .Ignore(p => p.Alerts); // Not mapped property

        } // Configure
    }
}