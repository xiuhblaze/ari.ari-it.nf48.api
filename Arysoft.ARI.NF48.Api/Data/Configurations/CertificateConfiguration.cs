using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class CertificateConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Certificate>()
                .ToTable("Certificates")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Certificate>()
                .Property(m => m.ID)
                .HasColumnName("CertificateID");

            modelBuilder.Entity<Certificate>()
                .Property(m => m.OrganizationID)
                .IsRequired();

            modelBuilder.Entity<Certificate>()
                .Property(m => m.Comments)
                .HasMaxLength(500);

            modelBuilder.Entity<Certificate>()
                .Property(m => m.Filename)
                .HasMaxLength(250);

            //modelBuilder.Entity<Certificate>()
            //    .Property(m => m.PrevAuditDate)
            //    .IsRequired();

            modelBuilder.Entity<Certificate>()
                .Property(m => m.PrevAuditNote)
                .HasMaxLength(100);

            modelBuilder.Entity<Certificate>()
                .Property(m => m.NextAuditNote)
                .HasMaxLength(100);

            modelBuilder.Entity<Certificate>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Certificate>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Certificate>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Certificate>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();

            // Relations

            modelBuilder.Entity<Certificate>()
                .HasMany(m => m.Notes)
                .WithOptional()
                .HasForeignKey(m => m.OwnerID);

            // Not Mapped

            modelBuilder.Entity<Certificate>()
                .Ignore(m => m.ValidityStatus);

            modelBuilder.Entity<Certificate>()
                .Ignore(m => m.AuditPlanValidityStatus);

        } // Configure
    }    
}