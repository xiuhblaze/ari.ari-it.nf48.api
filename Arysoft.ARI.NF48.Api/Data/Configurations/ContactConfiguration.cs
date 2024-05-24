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
                .HasMaxLength(100);

            modelBuilder.Entity<Contact>()
                .Property(m => m.Phone)
                .HasMaxLength(10);

            modelBuilder.Entity<Contact>()
                .Property(m => m.PhoneExtensions)
                .HasMaxLength(20);

            modelBuilder.Entity<Contact>()
                .Property(m => m.Email)
                .HasMaxLength(250);

            modelBuilder.Entity<Contact>()
                .Property(m => m.Position)
                .HasMaxLength(250);

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