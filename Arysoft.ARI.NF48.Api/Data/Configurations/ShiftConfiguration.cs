using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class ShiftConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shift>()
                .ToTable("Shifts")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Shift>()
                .Property(m => m.ID)
                .HasColumnName("ShiftID");

            modelBuilder.Entity<Shift>()
                .Property(m => m.ActivitiesDescription)
                .HasMaxLength(500);

            modelBuilder.Entity<Shift>()
                .Property(m => m.Status)
                .IsRequired();
        } // Configure
    }
}