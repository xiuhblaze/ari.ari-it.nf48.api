using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class AppFormConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        { 
            modelBuilder.Entity<AppForm>()
                .ToTable("AppForms")
                .HasKey(m => m.ID);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.ID)
                .HasColumnName("AppFormID");

            modelBuilder.Entity<AppForm>()
                .Property(m => m.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.ActivitiesScope)
                .HasMaxLength(1000);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.ProcessServicesDescription)
                .HasMaxLength(1000);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.LegalRequirements)
                .HasMaxLength(1000);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.CriticalComplaintComments)
                .HasMaxLength(1000);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.AutomationLevelJustification)
                .HasMaxLength(1000);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.DesignResponsibilityJustify)
                .HasMaxLength(1000);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.CurrentCertificationsExpiration)
                .HasMaxLength(100);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.CurrentStandards)
                .HasMaxLength(100);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.CurrentCertificationsBy)
                .HasMaxLength(100);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.OutsourcedProcess)
                .HasMaxLength(1000);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.AnyConsultancyBy)
                .HasMaxLength(250);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.SalesComments)
                .HasMaxLength(1000);

            //modelBuilder.Entity<AppForm>()
            //    .Property(m => m.ReviewJustification)
            //    .HasMaxLength(1000); // Va a ser MAX

            modelBuilder.Entity<AppForm>()
                .Property(m => m.ReviewComments)
                .HasMaxLength(1000);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.UserSales)
                .HasMaxLength(50);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.UserReviewer)
                .HasMaxLength(50);

            modelBuilder.Entity<AppForm>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<AppForm>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<AppForm>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<AppForm>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();

            // RELATIONS

            modelBuilder.Entity<AppForm>()
                .HasMany(m => m.Notes)
                .WithOptional()
                .HasForeignKey(m => m.OwnerID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<AppForm>() // Probar estas relaciones también XD
                .HasMany(m => m.Contacts)
                .WithMany()
                .Map(e => e.MapLeftKey("AppFormID")
                    .MapRightKey("ContactID")
                    .ToTable("AppFormsContacts"));

            modelBuilder.Entity<AppForm>()
                .HasMany(m => m.NaceCodes)
                .WithMany()
                .Map(e => e.MapLeftKey("AppFormID")
                    .MapRightKey("NaceCodeID")
                    .ToTable("AppFormsNaceCodes"));

            modelBuilder.Entity<AppForm>()
                .HasMany(m => m.Sites)
                .WithMany()
                .Map(e => e.MapLeftKey("AppFormID")
                    .MapRightKey("SiteID")
                    .ToTable("AppFormsSites"));

        } // Configure
    }
}