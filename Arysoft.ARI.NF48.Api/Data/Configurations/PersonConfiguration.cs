using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class PersonConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .ToTable("Persons")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Person>()
                .Property(m => m.ID)
                .HasColumnName("PersonID")
                .IsRequired();

            modelBuilder.Entity<Person>()
                .Property(m => m.FirstName)
                .HasMaxLength(50);

            modelBuilder.Entity<Person>()
                .Property(m => m.LastName)
                .HasMaxLength(50);

            modelBuilder.Entity<Person>()
                .Property(m => m.Email)
                .HasMaxLength(250);

            modelBuilder.Entity<Person>()
                .Property(m => m.Phone)
                .HasMaxLength(25);

            modelBuilder.Entity<Person>()
                .Property(m => m.PhoneAlt)
                .HasMaxLength(25);

            modelBuilder.Entity<Person>()
                .Property(m => m.LocationDescription)
                .HasMaxLength(1000);

            modelBuilder.Entity<Person>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Person>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Person>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Person>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}