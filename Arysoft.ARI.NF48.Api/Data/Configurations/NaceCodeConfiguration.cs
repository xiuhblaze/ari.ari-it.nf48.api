using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class NaceCodeConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NaceCode>()
                .HasKey(m => m.ID);

            modelBuilder.Entity<NaceCode>()
                .Property(m => m.ID)
                .HasColumnName("NaceCodeID");
        } // Configure
    }
}