using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class StandardConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Standard>()
                .ToTable("Standards")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Standard>()
                .Property(m => m.ID)
                .HasColumnName("StandardID");

            modelBuilder.Entity<Standard>()
                .Property(m => m.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<Standard>()
                .Property(m => m.Description)
                .HasMaxLength(250);

            modelBuilder.Entity<Standard>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Standard>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Standard>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Standard>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();

            // RELATIONS

            modelBuilder.Entity<Standard>()
                .HasMany<StandardTemplate>(m => m.StandardTemplates)
                .WithRequired(t => t.Standard)
                .HasForeignKey<Guid>(m => m.StandardID)
                .WillCascadeOnDelete(false);

        } // Configure
    }
}