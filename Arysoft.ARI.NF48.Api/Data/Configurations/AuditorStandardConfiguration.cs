using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class AuditorStandardConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditorStandard>()
                .ToTable("AuditorStandards")
                .HasKey(m => m.ID);

            modelBuilder.Entity<AuditorStandard>()
                .Property(m => m.ID)
                .HasColumnName("AuditorStandardID");

            modelBuilder.Entity<AuditorStandard>()
                .Property(m => m.AuditorID)
                .IsRequired();

            modelBuilder.Entity<AuditorStandard>()
                .Property(m => m.StandardID)
                .IsRequired();

            modelBuilder.Entity<AuditorStandard>()
                .Property(m => m.Comments)
                .HasMaxLength(1000);

            modelBuilder.Entity<AuditorStandard>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<AuditorStandard>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<AuditorStandard>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<AuditorStandard>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}