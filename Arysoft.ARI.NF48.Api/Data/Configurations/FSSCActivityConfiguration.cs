using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class FSSCActivityConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FSSCActivity>()
                .ToTable("FSSCActivities")
                .HasKey(e => e.ID);

            modelBuilder.Entity<FSSCActivity>()
                .Property(e => e.ID)
                .HasColumnName("FSSCActivityID");

            modelBuilder.Entity<FSSCActivity>()
                .Property(m => m.FSSCSubCategoryID)
                .IsRequired();

            modelBuilder.Entity<FSSCActivity>()
                .Property(e => e.Name)
                .HasMaxLength(200);

            modelBuilder.Entity<FSSCActivity>()
                .Property(e => e.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<FSSCActivity>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<FSSCActivity>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<FSSCActivity>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<FSSCActivity>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}