using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class ApplicationConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Application>()
                .ToTable("Applications")
                .HasKey(m => m.ID);

            modelBuilder.Entity<Application>()
                .Property(m => m.ID)
                .HasColumnName("ApplicationID");

            modelBuilder.Entity<Application>()
                .Property(m => m.ProcessScope)
                .HasMaxLength(1000);

            modelBuilder.Entity<Application>()
                .Property(m => m.Services)
                .HasMaxLength(1000);

            modelBuilder.Entity<Application>()
                .Property(m => m.LegalRequirements)
                .HasMaxLength(1000);

            modelBuilder.Entity<Application>()
                .Property(m => m.CriticalComplaintComments)
                .HasMaxLength(1000);

            modelBuilder.Entity<Application>()
                .Property(m => m.AutomationLevel)
                .HasMaxLength(1000);

            modelBuilder.Entity<Application>()
                .Property(m => m.DesignResponsibilityJustify)
                .HasMaxLength(1000);

            modelBuilder.Entity<Application>()
                .Property(m => m.CurrentCertificationBy)
                .HasMaxLength(50);

            modelBuilder.Entity<Application>()
                .Property(m => m.CurrentStandards)
                .HasMaxLength(250);

            modelBuilder.Entity<Application>()
                .Property(m => m.OutsourcedProcess)
                .HasMaxLength(1000);

            modelBuilder.Entity<Application>()
                .Property(m => m.AnyConsultancyBy)
                .HasMaxLength(250);

            modelBuilder.Entity<Application>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<Application>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<Application>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50);

            // RELATIONS

            modelBuilder.Entity<Application>() // Ver si es necesario
                .HasOptional(a => a.Organization);
            
        } // Configure
    }
}