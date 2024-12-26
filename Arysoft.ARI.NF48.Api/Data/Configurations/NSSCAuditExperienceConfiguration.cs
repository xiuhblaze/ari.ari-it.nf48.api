using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class NSSCAuditExperienceConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NSSCAuditExperience>()
                    .ToTable("NSSCAuditExperiences")
                    .HasKey(e => e.ID);

            modelBuilder.Entity<NSSCAuditExperience>()
                    .Property(e => e.ID)
                    .HasColumnName("NSSCAuditExperienceID");

            modelBuilder.Entity<NSSCAuditExperience>()
                    .Property(e => e.NSSCAuditorActivityID)
                    .IsRequired();

            modelBuilder.Entity<NSSCAuditExperience>()
                    .Property(e => e.Description)
                    .HasMaxLength(1000);

            modelBuilder.Entity<NSSCAuditExperience>()
                    .Property(m => m.Status)
                    .IsRequired();

            modelBuilder.Entity<NSSCAuditExperience>()
                    .Property(m => m.Created)
                    .IsRequired();

            modelBuilder.Entity<NSSCAuditExperience>()
                    .Property(m => m.Updated)
                    .IsRequired();

            modelBuilder.Entity<NSSCAuditExperience>()
                    .Property(m => m.UpdatedUser)
                    .HasMaxLength(50)
                    .IsRequired();
        }
    }
}