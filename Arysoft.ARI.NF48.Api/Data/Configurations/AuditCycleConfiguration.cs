using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class AuditCycleConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        { 
            modelBuilder.Entity<AuditCycle>()
                .ToTable("AuditCycles")
                .HasKey(m => m.ID);

            modelBuilder.Entity<AuditCycle>()
                .Property(m => m.ID)
                .HasColumnName("AuditCycleID");

            modelBuilder.Entity<AuditCycle>()
                .Property(m => m.Name)
                .HasMaxLength(50);

            modelBuilder.Entity<AuditCycle>()
                .Property(m => m.ExtraInfo)
                .HasMaxLength(1000);

            modelBuilder.Entity<AuditCycle>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<AuditCycle>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<AuditCycle>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<AuditCycle>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();

            // RELATIONS

            // xBlaze: No se puede crear una auditoria si el ciclo es temporal
            //         y no se puede eliminar un ciclo que no sea temporal
            //modelBuilder.Entity<AuditCycle>()
            //    .HasMany(ac => ac.Audits)
            //    .WithRequired(a => a.AuditCycle)
            //    .HasForeignKey(a => a.AuditCycleID)
            //    .WillCascadeOnDelete(true);

            //modelBuilder.Entity<AuditCycle>()
            //    .HasMany(ac => ac.AuditCycleStandards)
            //    .WithRequired(acs => acs.AuditCycle)
            //    .HasForeignKey(acs => acs.AuditCycleID)
            //    .WillCascadeOnDelete(true);

            //modelBuilder.Entity<AuditCycle>() // xBlaze: Cambió la estructura de datos por: muchos a muchos
            //    .HasMany(ac => ac.AuditCycleDocuments)
            //    .WithRequired(acd => acd.AuditCycle)
            //    .HasForeignKey(acd => acd.AuditCycleID)
            //    .WillCascadeOnDelete(true);

            modelBuilder.Entity<AuditCycle>()
                .HasMany(ac => ac.AuditCycleDocuments)
                .WithMany(acd => acd.AuditCycles)
                .Map(e => e.MapLeftKey("AuditCycleID")
                    .MapRightKey("AuditCycleDocumentID")
                    .ToTable("AuditCycleDocumentsStandards"));

        }
    }
}