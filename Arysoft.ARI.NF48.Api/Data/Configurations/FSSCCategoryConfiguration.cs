using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class FSSCCategoryConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FSSCCategory>()
                .ToTable("FSSCCategories")
                .HasKey(e => e.ID);

            modelBuilder.Entity<FSSCCategory>()
                .Property(e => e.ID)
                .HasColumnName("FSSCCategoryID");

            modelBuilder.Entity<FSSCCategory>()
                .Property(e => e.Name)
                .HasMaxLength(50);

            modelBuilder.Entity<FSSCCategory>()
                .Property(e => e.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<FSSCCategory>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<FSSCCategory>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<FSSCCategory>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<FSSCCategory>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}