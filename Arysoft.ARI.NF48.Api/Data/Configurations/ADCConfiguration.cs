using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class ADCConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ADC>()
                .ToTable("ADCs")
                .HasKey(m => m.ID);

            modelBuilder.Entity<ADC>()
                .Property(m => m.ID)
                .HasColumnName("ADCID");

            modelBuilder.Entity<ADC>()
                .Property(m => m.AuditCycleID)
                .IsRequired();

            modelBuilder.Entity<ADC>()
                .Property(m => m.AppFormID)
                .IsRequired();

            modelBuilder.Entity<ADC>()
                .Property(m => m.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<ADC>()
                .Property(m => m.ExtraInfo)
                .HasMaxLength(500);

            modelBuilder.Entity<ADC>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<ADC>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<ADC>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();

            // RELATIONS

            modelBuilder.Entity<ADC>()
                .HasMany(a => a.ADCSites)
                .WithRequired(s => s.ADC)
                .HasForeignKey(s => s.ADCID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ADC>()
                .HasMany(m => m.Notes)
                .WithOptional()
                .HasForeignKey(m => m.OwnerID)
                .WillCascadeOnDelete(true);

            // NOT MAPPED

            modelBuilder.Entity<ADC>()
                .Ignore(m => m.Alerts); // Not mapped property
        }
    }
}