using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class NSSCActivityConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NSSCActivity>()
                .ToTable("NSSCActivities")
                .HasKey(e => e.ID);

            modelBuilder.Entity<NSSCActivity>()
                .Property(e => e.ID)
                .HasColumnName("NSSCActivityID");

            modelBuilder.Entity<NSSCActivity>()
                .Property(m => m.NSSCSubCategoryID)
                .IsRequired();

            modelBuilder.Entity<NSSCActivity>()
                .Property(e => e.Name)
                .HasMaxLength(200);

            modelBuilder.Entity<NSSCActivity>()
                .Property(e => e.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<NSSCActivity>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<NSSCActivity>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<NSSCActivity>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<NSSCActivity>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}