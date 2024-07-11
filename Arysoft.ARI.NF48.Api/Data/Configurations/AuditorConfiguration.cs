using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class AuditorConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auditor>()
                .ToTable("Auditors")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Auditor>()
                .Property(m => m.ID)
                .HasColumnName("AuditorID");

            modelBuilder.Entity<Auditor>()
                .Property(m => m.Status)
                .IsRequired();


        }
    }
}