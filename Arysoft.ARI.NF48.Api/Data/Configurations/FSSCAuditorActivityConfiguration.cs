using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class FSSCAuditorActivityConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FSSCAuditorActivity>()
                .ToTable("FSSCAuditorActivities")
                .HasKey(x => x.ID);

            modelBuilder.Entity<FSSCAuditorActivity>()
                .Property(x => x.ID)
                .HasColumnName("FSSCAuditorActivityID");

            modelBuilder.Entity<FSSCAuditorActivity>()
                .Property(x => x.AuditorID)
                .IsRequired();

            modelBuilder.Entity<FSSCAuditorActivity>()
                .Property(x => x.FSSCActivityID)
                .IsRequired();

            modelBuilder.Entity<FSSCAuditorActivity>()
                .Property(x => x.Education)
                .HasMaxLength(1000);

            modelBuilder.Entity<FSSCAuditorActivity>()
                .Property(x => x.LegalRequirements)
                .HasMaxLength(1000);

            modelBuilder.Entity<FSSCAuditorActivity>()
                .Property(x => x.SpecificTraining)
                .HasMaxLength(1000);

            modelBuilder.Entity<FSSCAuditorActivity>()
                .Property(x => x.Comments)
                .HasMaxLength(1000);

            modelBuilder.Entity<FSSCActivity>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<FSSCActivity>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<FSSCActivity>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<FSSCActivity>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}