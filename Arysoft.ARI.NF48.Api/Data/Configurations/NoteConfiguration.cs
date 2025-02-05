using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class NoteConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Note>()
                .ToTable("Notes")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Note>()
                .Property(m => m.ID)
                .HasColumnName("NoteID");

            modelBuilder.Entity<Note>()
                .Property(m => m.OwnerID)
                .IsRequired();

            modelBuilder.Entity<Note>()
                .Property(m => m.Text)
                .HasMaxLength(250);

            modelBuilder.Entity<Note>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Note>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Note>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Note>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();

            // Relations

            //modelBuilder.Entity<Note>()
            //    .HasOptional(m => m.Owner)
            //    .WithMany(m => m.Notes)
            //    .HasForeignKey(m => m.OwnerID);
        }
    }
}