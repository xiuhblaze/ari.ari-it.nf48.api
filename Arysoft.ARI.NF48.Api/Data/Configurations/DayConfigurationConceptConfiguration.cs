using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class DayConfigurationConceptConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DayCalculationConcept>()
                .ToTable("DayCalculationConcepts")
                .HasKey(m => m.ID);

            modelBuilder.Entity<DayCalculationConcept>()
                .Property(m => m.ID)
                .HasColumnName("DayCalculationConceptID");

            modelBuilder.Entity<DayCalculationConcept>()
                .Property(m => m.Description)
                .HasMaxLength(100);

            modelBuilder.Entity<DayCalculationConcept>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<DayCalculationConcept>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<DayCalculationConcept>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<DayCalculationConcept>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50);
        }
    }
}