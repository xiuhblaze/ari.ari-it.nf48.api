using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class AuditCycleDocumentConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditCycleDocument>()
                .ToTable("AuditCycleDocuments")
                .HasKey(m => m.ID);

            modelBuilder.Entity<AuditCycleDocument>()
                .Property(m => m.ID)
                .HasColumnName("AuditCycleDocumentID");

            modelBuilder.Entity<AuditCycleDocument>()
                .Property(m => m.AuditCycleID)
                .IsRequired();

            modelBuilder.Entity<AuditCycleDocument>()
                .Property(m => m.Filename)
                .HasMaxLength(250);

            modelBuilder.Entity<AuditCycleDocument>()
                .Property(m => m.Version)
                .HasMaxLength(10);

            modelBuilder.Entity<AuditCycleDocument>()
                .Property(m => m.Comments)
                .HasMaxLength(500);

            modelBuilder.Entity<AuditCycleDocument>()
                .Property(m => m.OtherDescription)
                .HasMaxLength(100);

            modelBuilder.Entity<AuditCycleDocument>()
                .Property(m => m.UploadedBy)
                .HasMaxLength(50);

            modelBuilder.Entity<AuditCycleDocument>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<AuditCycleDocument>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<AuditCycleDocument>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<AuditCycleDocument>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}