using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class DayCalculationConceptApplicationConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DayCalculationConceptApplication>()
                .ToTable("DayCalculationConceptsApplications")
                .HasKey(m => m.ID);

            modelBuilder.Entity<DayCalculationConceptApplication>()
                .Property(m => m.ID)
                .HasColumnName("DayCalculationConceptApplicationID");

            modelBuilder.Entity<DayCalculationConceptApplication>()
                .Property(m => m.DayCalculationConceptID)
                .IsRequired();

            modelBuilder.Entity<DayCalculationConceptApplication>()
                .Property(m => m.ApplicationID)
                .IsRequired();

            modelBuilder.Entity<DayCalculationConceptApplication>()
                .Property(m => m.Justification)
                .HasMaxLength(1000);

            modelBuilder.Entity<DayCalculationConceptApplication>()
                .Property(m => m.JustificationApproved)
                .HasMaxLength(1000);

            modelBuilder.Entity<DayCalculationConceptApplication>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<DayCalculationConceptApplication>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<DayCalculationConceptApplication>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<DayCalculationConceptApplication>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50);
        }
    }
}