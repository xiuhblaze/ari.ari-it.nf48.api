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
                .Property(m => m.PersonID)
                .IsRequired();

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
        }
    }
}