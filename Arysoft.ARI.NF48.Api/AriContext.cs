using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api
{
    public class AriContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<NaceCode> NaceCodes { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Standard> Standards { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public AriContext() : base("DefaultConnection") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Role>()
                .Property(m => m.ID)
                .HasColumnName("RoleID");

            modelBuilder.Entity<User>()
                .Property(m => m.ID)
                .HasColumnName("UserID");

            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles).WithMany(r => r.Users)
                .Map(e => e.MapLeftKey("UserID")
                    .MapRightKey("RoleID")
                    .ToTable("RolesUsers")
                );
        }
    }
}