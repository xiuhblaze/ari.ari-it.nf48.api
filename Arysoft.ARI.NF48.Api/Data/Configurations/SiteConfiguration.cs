using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class SiteConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Site>()
                .ToTable("Sites")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Site>()
                .Property(m => m.ID)
                .HasColumnName("SiteID");

            modelBuilder.Entity<Site>()
                .Property(m => m.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<Site>()
                .Property(m => m.LocationDescription)
                .HasMaxLength(500);

            modelBuilder.Entity<Site>()
                .Property(m => m.Order)
                .IsRequired();

            modelBuilder.Entity<Site>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Site>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Site>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Site>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50);

            // RELATIONS

            //modelBuilder.Entity<Site>()
            //    .HasMany(s => s.Shifts)                
            //    .WithOptional(s => s.Site)
            //    .HasForeignKey(s => s.SiteID);

        } // Configure
    }
}