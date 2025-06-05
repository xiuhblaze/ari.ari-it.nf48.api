using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class ADCConceptValueConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ADCConceptValue>()
                .ToTable("ADCConceptValues")
                .HasKey(m => m.ID);

            modelBuilder.Entity<ADCConceptValue>()
                .Property(m => m.ID)
                .HasColumnName("ADCConceptValueID");

            modelBuilder.Entity<ADCConceptValue>()
                .Property(m => m.ADCConceptID)
                .IsRequired();

            modelBuilder.Entity<ADCConceptValue>()
                .Property(m => m.ADCSiteID)
                .IsRequired();

            modelBuilder.Entity<ADCConceptValue>()
                .Property(m => m.Justification)
                .HasMaxLength(500);

            modelBuilder.Entity<ADCConceptValue>()
                .Property(m => m.JustificationApproved)
                .HasMaxLength(500);

            modelBuilder.Entity<ADCConceptValue>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<ADCConceptValue>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<ADCConceptValue>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<ADCConceptValue>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}