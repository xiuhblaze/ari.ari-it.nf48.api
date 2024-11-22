using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class ContactConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>()
                .ToTable("Contacts")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Contact>()
                .Property(m => m.ID)
                .HasColumnName("ContactID");

            modelBuilder.Entity<Contact>()
                .Property(m => m.FirstName)
                .HasMaxLength(50);

            modelBuilder.Entity<Contact>()
                .Property(m => m.MiddleName)
                .HasMaxLength(50);

            modelBuilder.Entity<Contact>()
                .Property(m => m.LastName)
                .HasMaxLength(50);

            modelBuilder.Entity<Contact>()
                .Property(m => m.Email)
                .HasMaxLength(250);

            modelBuilder.Entity<Contact>()
                .Property(m => m.Phone)
                .HasMaxLength(25);

            modelBuilder.Entity<Contact>()
                .Property(m => m.PhoneAlt)
                .HasMaxLength(25);

            modelBuilder.Entity<Contact>()
                .Property(m => m.Address)
                .HasMaxLength(500);

            modelBuilder.Entity<Contact>()
                .Property(m => m.Position)
                .HasMaxLength(250);

            modelBuilder.Entity<Contact>()
                .Property(m => m.PhotoFilename)
                .HasMaxLength(250);

            modelBuilder.Entity<Contact>()
                .Property(m => m.IsMainContact)
                .IsRequired();

            modelBuilder.Entity<Contact>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Contact>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Contact>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Contact>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();

        } // Configure
    }
}