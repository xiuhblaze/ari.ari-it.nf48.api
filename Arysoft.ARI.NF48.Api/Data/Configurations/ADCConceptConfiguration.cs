using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class ADCConceptConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ADCConcept>()
                .ToTable("ADCConcepts")
                .HasKey(m => m.ID);

            modelBuilder.Entity<ADCConcept>()
                .Property(m => m.ID)
                .HasColumnName("ADCConceptID");

            modelBuilder.Entity<ADCConcept>()
                .Property(m => m.StandardID)
                .IsRequired();

            modelBuilder.Entity<ADCConcept>()
                .Property(m => m.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<ADCConcept>()
                .Property(m => m.Increase)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ADCConcept>()
                .Property(m => m.Decrease)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ADCConcept>()
                .Property(m => m.ExtraInfo)
                .HasMaxLength(500);

            modelBuilder.Entity<ADCConcept>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<ADCConcept>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<ADCConcept>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<ADCConcept>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}