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
                .Property(m => m.AppFormID)
                .IsRequired();

            modelBuilder.Entity<Proposal>()
                .Property(m => m.ADCID)
                .IsRequired();

            modelBuilder.Entity<Proposal>()
                .Property(m => m.ActivitiesScope)
                .HasMaxLength(1000);

            modelBuilder.Entity<Proposal>()
                .Property(m => m.SignerName)
                .HasMaxLength(150);

            modelBuilder.Entity<Proposal>()
                .Property(m => m.SignerPosition)
                .HasMaxLength(100);

            modelBuilder.Entity<Proposal>()
                .Property(m => m.SigendFilename)
                .HasMaxLength(250);

            modelBuilder.Entity<Proposal>()
                .Property(m => m.UserCreates)
                .HasMaxLength(50);

            modelBuilder.Entity<Proposal>()
                .Property(m => m.UserReview)
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

            //modelBuilder.Entity<Proposal>()
            //    .HasMany(m => m.ProposalSites)
            //    .WithRequired(m => m.Proposal)
            //    .HasForeignKey(m => m.ProposalID)
            //    .WillCascadeOnDelete(true);

            modelBuilder.Entity<AppForm>()
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