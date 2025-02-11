using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class CatAuditorDocumentConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CatAuditorDocument>()
                .ToTable("CatAuditorDocuments")
                .HasKey(m => m.ID);

            modelBuilder.Entity<CatAuditorDocument>()
                .Property(m => m.ID)
                .HasColumnName("CatAuditorDocumentID");

            modelBuilder.Entity<CatAuditorDocument>()
                .Property(m => m.Name)
                .HasMaxLength(50);

            modelBuilder.Entity<CatAuditorDocument>()
                .Property(m => m.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<CatAuditorDocument>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<CatAuditorDocument>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<CatAuditorDocument>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<CatAuditorDocument>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}