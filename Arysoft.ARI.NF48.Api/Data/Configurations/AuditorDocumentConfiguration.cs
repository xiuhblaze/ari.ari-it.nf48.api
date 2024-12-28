using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class AuditorDocumentConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditorDocument>()
                .ToTable("AuditorDocuments")
                .HasKey(m => m.ID);

            modelBuilder.Entity<AuditorDocument>()
                .Property(m => m.ID)
                .HasColumnName("AuditorDocumentID");

            modelBuilder.Entity<AuditorDocument>()
                .Property(m => m.AuditorID)
                .IsRequired();

            modelBuilder.Entity<AuditorDocument>()
                .Property(m => m.CatAuditorDocumentID)
                .IsRequired();

            modelBuilder.Entity<AuditorDocument>()
                .Property(m => m.Filename)
                .HasMaxLength(250);

            modelBuilder.Entity<AuditorDocument>()
                .Property(m => m.Observations)
                .HasMaxLength(500);

            modelBuilder.Entity<AuditorDocument>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<AuditorDocument>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<AuditorDocument>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<AuditorDocument>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}