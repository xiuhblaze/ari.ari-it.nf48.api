using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class RiskLevelConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.RiskLevel>()
                .ToTable("RiskLevels")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Models.RiskLevel>()
                .Property(m => m.ID)
                .HasColumnName("RiskLevelID");

            modelBuilder.Entity<Models.RiskLevel>()
                .Property(m => m.BusinessSector)
                .HasMaxLength(1000);
        }
    }
}