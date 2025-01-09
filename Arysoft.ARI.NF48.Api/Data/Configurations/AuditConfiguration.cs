using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class AuditConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        { 
            modelBuilder.Entity<Audit>()
                .ToTable("Audits")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Audit>()
                .Property(m => m.ID)
                .HasColumnName("AuditID");

            modelBuilder.Entity<Audit>()
                .Property(m => m.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<Audit>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Audit>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Audit>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Audit>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Audit>() // Ver si jala esto
                .HasMany(a => a.Notes)
                .WithOptional()
                .HasForeignKey(n => n.OwnerID);
        }
    }
}