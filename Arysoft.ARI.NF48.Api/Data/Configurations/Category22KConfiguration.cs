using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class Category22KConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category22K>()
                .ToTable("Categories22K")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Category22K>()
                .Property(m => m.ID)
                .HasColumnName("Category22KID");

            modelBuilder.Entity<Category22K>()
                .Property(m => m.Cluster)
                .HasMaxLength(50);

            modelBuilder.Entity<Category22K>()
                .Property(m => m.Category)
                .HasMaxLength(5);

            modelBuilder.Entity<Category22K>()
                .Property(m => m.CategoryDescription)
                .HasMaxLength(100);

            modelBuilder.Entity<Category22K>()
                .Property(m => m.SubCategory)
                .HasMaxLength(5);

            modelBuilder.Entity<Category22K>()
                .Property(m => m.SubCategoryDescription)
                .HasMaxLength(100);

            modelBuilder.Entity<Category22K>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Category22K>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Category22K>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Category22K>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50);
        }
    }
}