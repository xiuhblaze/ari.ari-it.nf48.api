using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class NSSCAuditorActivityConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NSSCAuditorActivity>()
                .ToTable("NSSCAuditorActivities")
                .HasKey(x => x.ID);

            modelBuilder.Entity<NSSCAuditorActivity>()
                .Property(x => x.ID)
                .HasColumnName("NSSCAuditorActivityID");

            modelBuilder.Entity<NSSCAuditorActivity>()
                .Property(x => x.AuditorID)
                .IsRequired();

            modelBuilder.Entity<NSSCAuditorActivity>()
                .Property(x => x.NSSCActivityID)
                .IsRequired();

            modelBuilder.Entity<NSSCAuditorActivity>()
                .Property(x => x.Education)
                .HasMaxLength(1000);

            modelBuilder.Entity<NSSCAuditorActivity>()
                .Property(x => x.LegalRequirements)
                .HasMaxLength(1000);

            modelBuilder.Entity<NSSCAuditorActivity>()
                .Property(x => x.SpecificTraining)
                .HasMaxLength(1000);

            modelBuilder.Entity<NSSCAuditorActivity>()
                .Property(x => x.Comments)
                .HasMaxLength(1000);

            modelBuilder.Entity<NSSCActivity>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<NSSCActivity>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<NSSCActivity>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<NSSCActivity>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}