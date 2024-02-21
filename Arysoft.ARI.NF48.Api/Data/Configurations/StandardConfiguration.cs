using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class StandardConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Standard>()
                .HasKey(m => m.ID);

            modelBuilder.Entity<Standard>()
                .Property(m => m.ID)
                .HasColumnName("StandardID");
        } // Configure
    }
}