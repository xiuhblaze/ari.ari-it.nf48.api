using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class ContactConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>()
                .HasKey(m => m.ID);

            modelBuilder.Entity<Contact>()
                .Property(m => m.ID)
                .HasColumnName("ContactID");
        } // Configure
    }
}