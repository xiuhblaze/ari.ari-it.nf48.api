using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

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
                .Property(m => m.Days)
                .HasMaxLength(5);

            modelBuilder.Entity<Audit>()
                .Property(m => m.ExtraInfo)
                .HasMaxLength(1000);

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

            // RELATIONS

            modelBuilder.Entity<Audit>()
                .HasMany(a => a.Sites)
                .WithMany(s => s.Audits)
                .Map(e => e.MapLeftKey("AuditID")
                    .MapRightKey("SiteID")
                    .ToTable("AuditSites"));

            // - Para que se borren en cascada, no jaló por su complejidad
            //   en AuditRepository.Delete() se encuentran las instrucciones
            //   para borrar en cascada los registros de las tablas intermedias

            modelBuilder.Entity<Audit>()
                .HasMany(a => a.Notes)
                .WithOptional()
                .HasForeignKey(n => n.OwnerID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Audit>()
                .HasMany(a => a.AuditAuditors)
                .WithRequired(aa => aa.Audit)
                .HasForeignKey(a => a.AuditID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Audit>()
                .HasMany(a => a.AuditStandards)
                .WithRequired(asd => asd.Audit)
                .HasForeignKey(a => a.AuditID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Audit>()
                .HasMany(a => a.AuditDocuments)
                .WithRequired(ad => ad.Audit)
                .HasForeignKey(a => a.AuditID)
                .WillCascadeOnDelete(false);
        }
    }
}