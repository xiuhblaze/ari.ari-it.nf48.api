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
                .Property(m => m.MD11Filename)
                .HasMaxLength(250);

            modelBuilder.Entity<ADCSite>()
                .Property(m => m.MD11UploadedBy)
                .HasMaxLength(50);

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

            // NOT MAPPED

            modelBuilder.Entity<ADCSite>()
                .Ignore(m => m.Alerts); // Not mapped property
        }
    }
}