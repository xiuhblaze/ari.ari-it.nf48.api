using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class NSSCCategoryConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NSSCCategory>()
                .ToTable("NSSCCategories")
                .HasKey(e => e.ID);

            modelBuilder.Entity<NSSCCategory>()
                .Property(e => e.ID)
                .HasColumnName("NSSCCategoryID");

            modelBuilder.Entity<NSSCCategory>()
                .Property(e => e.Name)
                .HasMaxLength(50);

            modelBuilder.Entity<NSSCCategory>()
                .Property(e => e.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<NSSCCategory>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<NSSCCategory>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<NSSCCategory>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<NSSCCategory>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}