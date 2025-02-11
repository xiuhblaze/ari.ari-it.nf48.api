using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class FSSCJobExperienceConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        { 
            modelBuilder.Entity<FSSCJobExperience>()
                .ToTable("FSSCJobExperiences")
                .HasKey(e => e.ID);

            modelBuilder.Entity<FSSCJobExperience>()
                .Property(e => e.ID)
                .HasColumnName("FSSCJobExperienceID");

            modelBuilder.Entity<FSSCJobExperience>()
                .Property(e => e.FSSCAuditorActivityID)
                .IsRequired();

            modelBuilder.Entity<FSSCJobExperience>()
                .Property(e => e.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<FSSCJobExperience>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<FSSCJobExperience>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<FSSCJobExperience>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<FSSCJobExperience>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}