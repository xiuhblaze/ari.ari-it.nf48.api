using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class NSSCSubCategoryConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NSSCSubCategory>()
                .ToTable("NSSCSubCategories")
                .HasKey(e => e.ID);

            modelBuilder.Entity<NSSCSubCategory>()
                .Property(e => e.ID)
                .HasColumnName("NSSCSubCategoryID");

            modelBuilder.Entity<NSSCSubCategory>()
                .Property(m => m.NSSCCategoryID)
                .IsRequired();

            modelBuilder.Entity<NSSCSubCategory>()
                .Property(e => e.Name)
                .HasMaxLength(50);

            modelBuilder.Entity<NSSCSubCategory>()
                .Property(e => e.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<NSSCSubCategory>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<NSSCSubCategory>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<NSSCSubCategory>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<NSSCSubCategory>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}