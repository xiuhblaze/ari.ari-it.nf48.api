using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class CalendarBlockConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        { 
            modelBuilder.Entity<Models.CalendarBlock>()
                .ToTable("CalendarBlocks")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Models.CalendarBlock>()
                .Property(m => m.ID)
                .HasColumnName("CalendarBlockID");
            
            modelBuilder.Entity<Models.CalendarBlock>()
                .Property(m => m.Description)
                .HasMaxLength(250);

            modelBuilder.Entity<Models.CalendarBlock>()
                .Property(m => m.StartDate)
                .HasColumnType("date");

            modelBuilder.Entity<Models.CalendarBlock>()
                .Property(m => m.EndDate)
                .HasColumnType("date");

            modelBuilder.Entity<Models.CalendarBlock>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Models.CalendarBlock>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Models.CalendarBlock>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Models.CalendarBlock>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}