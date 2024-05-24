using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class ShiftConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shift>()
                .ToTable("Shifts")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Shift>()
                .Property(m => m.ID)
                .HasColumnName("ShiftID");

            modelBuilder.Entity<Shift>()
                .Property(m => m.ActivitiesDescription)
                .HasMaxLength(500);

            modelBuilder.Entity<Shift>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Shift>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Shift>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Shift>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        } // Configure
    }
}