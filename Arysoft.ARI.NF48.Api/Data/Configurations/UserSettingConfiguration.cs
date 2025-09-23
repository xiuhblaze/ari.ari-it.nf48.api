using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class UserSettingConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.UserSetting>()
                .ToTable("UserSettings")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Models.UserSetting>()
                .Property(m => m.ID)
                .HasColumnName("UserSettingID");

            modelBuilder.Entity<Models.UserSetting>()
                .Property(m => m.UserID)
                .IsRequired();

            modelBuilder.Entity<Models.UserSetting>()
                .Property(m => m.Settings)
                .HasMaxLength(1000);
            
            modelBuilder.Entity<Models.UserSetting>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<Models.UserSetting>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Models.UserSetting>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Models.UserSetting>()
                .Property(m => m.UpdatedUser)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}