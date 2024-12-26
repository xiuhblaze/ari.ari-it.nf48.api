using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class NSSCJobExperienceConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        { 
            modelBuilder.Entity<NSSCJobExperience>()
                .ToTable("NSSCJobExperiences")
                .HasKey(e => e.ID);

            modelBuilder.Entity<NSSCJobExperience>()
                .Property(e => e.ID)
                .HasColumnName("NSSCJobExperienceID");

            modelBuilder.Entity<NSSCJobExperience>()
                .Property(e => e.NSSCAuditorActivityID)
                .IsRequired();

            modelBuilder.Entity<NSSCJobExperience>()
                .Property(e => e.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<NSSCJobExperience>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<NSSCJobExperience>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<NSSCJobExperience>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<NSSCJobExperience>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}