using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class AuditDocumentConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditDocument>()
                .ToTable("AuditDocuments")
                .HasKey(m => m.ID);

            modelBuilder.Entity<AuditDocument>()
                .Property(m => m.ID)
                .HasColumnName("AuditDocumentID");

            modelBuilder.Entity<AuditDocument>()
                .Property(m => m.AuditID)
                .IsRequired();

            modelBuilder.Entity<AuditDocument>()
                .Property(m => m.Filename)
                .HasMaxLength(255);

            modelBuilder.Entity<AuditDocument>()
                .Property(m => m.Comments)
                .HasMaxLength(500);

            modelBuilder.Entity<AuditDocument>()
                .Property(m => m.OtherDescription)
                .HasMaxLength(100);

            modelBuilder.Entity<AuditDocument>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<AuditDocument>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<AuditDocument>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<AuditDocument>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}