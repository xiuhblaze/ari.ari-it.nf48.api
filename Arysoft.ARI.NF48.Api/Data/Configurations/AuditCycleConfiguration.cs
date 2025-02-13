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
        }
    }
}