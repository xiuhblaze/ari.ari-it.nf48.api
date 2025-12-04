using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class ADCSiteConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ADCSite>()
                .ToTable("ADCSites")
                .HasKey(m => m.ID);

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.ID)
                .HasColumnName("ADCSiteID");

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.ADCID)
                .IsRequired();

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.InitialMD5)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.TotalInitial)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.MD11)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.MD11Filename)
                .HasMaxLength(250);

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.MD11UploadedBy)
                .HasMaxLength(50);

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.Total)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.Surveillance)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.Recertification)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.ExtraInfo)
                .HasMaxLength(500);

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();

            // RELATIONS

            modelBuilder.Entity<ADCSite>()
                .HasMany(s => s.ADCConceptValues)
                .WithRequired(cv => cv.ADCSite)
                .HasForeignKey(cv => cv.ADCSiteID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ADCSite>()
                .HasMany(s => s.ADCSiteAudits)
                .WithRequired(a => a.ADCSite)
                .HasForeignKey(a => a.ADCSiteID)
                .WillCascadeOnDelete(true);

            // NOT MAPPED

            modelBuilder.Entity<ADCSite>()
                .Ignore(m => m.Alerts); // Not mapped property

            modelBuilder.Entity<ADCSite>()
                .Ignore(m => m.IsMultiStandard); // Not mapped property
        }
    }
}