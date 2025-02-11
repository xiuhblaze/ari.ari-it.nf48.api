using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class FSSCAuditExperienceConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FSSCAuditExperience>()
                    .ToTable("FSSCAuditExperiences")
                    .HasKey(e => e.ID);

            modelBuilder.Entity<FSSCAuditExperience>()
                    .Property(e => e.ID)
                    .HasColumnName("FSSCAuditExperienceID");

            modelBuilder.Entity<FSSCAuditExperience>()
                    .Property(e => e.FSSCAuditorActivityID)
                    .IsRequired();

            modelBuilder.Entity<FSSCAuditExperience>()
                    .Property(e => e.Description)
                    .HasMaxLength(1000);

            modelBuilder.Entity<FSSCAuditExperience>()
                    .Property(m => m.Status)
                    .IsRequired();

            modelBuilder.Entity<FSSCAuditExperience>()
                    .Property(m => m.Created)
                    .IsRequired();

            modelBuilder.Entity<FSSCAuditExperience>()
                    .Property(m => m.Updated)
                    .IsRequired();

            modelBuilder.Entity<FSSCAuditExperience>()
                    .Property(m => m.UpdatedUser)
                    .HasMaxLength(50)
                    .IsRequired();
        }
    }
}