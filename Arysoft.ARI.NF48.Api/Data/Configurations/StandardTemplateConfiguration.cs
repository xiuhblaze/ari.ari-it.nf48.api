using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class StandardTemplateConfiguration
    {
        public static void Configure(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.StandardTemplate>()
                .ToTable("StandardTemplates")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Models.StandardTemplate>()
                .Property(m => m.ID)
                .HasColumnName("StandardTemplateID");

            modelBuilder.Entity<Models.StandardTemplate>()
                .Property(m => m.StandardID)
                .IsRequired();

            modelBuilder.Entity<Models.StandardTemplate>()
                .Property(m => m.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<Models.StandardTemplate>()
                .Property(m => m.Filename)
                .HasMaxLength(250);

            modelBuilder.Entity<Models.StandardTemplate>()
                .Property(m => m.Version)
                .HasMaxLength(10);

            modelBuilder.Entity<Models.StandardTemplate>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Models.StandardTemplate>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Models.StandardTemplate>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Models.StandardTemplate>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();

        } // Configure
    }
}