using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class FSSCSubCategoryConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FSSCSubCategory>()
                .ToTable("FSSCSubCategories")
                .HasKey(e => e.ID);

            modelBuilder.Entity<FSSCSubCategory>()
                .Property(e => e.ID)
                .HasColumnName("FSSCSubCategoryID");

            modelBuilder.Entity<FSSCSubCategory>()
                .Property(m => m.FSSCCategoryID)
                .IsRequired();

            modelBuilder.Entity<FSSCSubCategory>()
                .Property(e => e.Name)
                .HasMaxLength(50);

            modelBuilder.Entity<FSSCSubCategory>()
                .Property(e => e.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<FSSCSubCategory>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<FSSCSubCategory>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<FSSCSubCategory>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<FSSCSubCategory>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}