using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

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
                .Property(m => m.FirstName)
                .HasMaxLength(50);

            modelBuilder.Entity<Auditor>()
                .Property(m => m.MiddleName)
                .HasMaxLength(50);

            modelBuilder.Entity<Auditor>()
                .Property(m => m.LastName)
                .HasMaxLength(50);

            modelBuilder.Entity<Auditor>()
                .Property(m => m.Email)
                .HasMaxLength(250);

            modelBuilder.Entity<Auditor>()
                .Property(m => m.Phone)
                .HasMaxLength(25);

            modelBuilder.Entity<Auditor>()
                .Property(m => m.Address)
                .HasMaxLength(500);

            modelBuilder.Entity<Auditor>()
                .Property(m => m.PhotoFilename)
                .HasMaxLength(250);

            modelBuilder.Entity<Auditor>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Auditor>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Auditor>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Auditor>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();

            // Not Mapped

            modelBuilder.Entity<Auditor>()
                .Ignore(m => m.ValidityStatus);

            modelBuilder.Entity<Auditor>()
                .Ignore(m => m.RequiredStatus);
        }
    }
}