using Arysoft.ARI.NF48.Api.Data.Configurations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Arysoft.ARI.NF48.Api.Data
{
    public class AriContext : DbContext
    {
        public AriContext() : base("DefaultConnection") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Módulo para actualización del proceso de auditorias
            ApplicationConfiguration.Configure(modelBuilder);
            DayCalculationConceptConfiguration.Configure(modelBuilder);
            DayCalculationConceptApplicationConfiguration.Configure(modelBuilder);
            
            // Módulo de auditores 
            AuditorConfiguration.Configure(modelBuilder);
            AuditorDocumentConfiguration.Configure(modelBuilder);
            AuditorStandardConfiguration.Configure(modelBuilder);
            CatAuditorDocumentConfiguration.Configure(modelBuilder);

            // Módulo de documentción para FSSC
            FSSCCategoryConfiguration.Configure(modelBuilder);
            FSSCSubCategoryConfiguration.Configure(modelBuilder);
            FSSCActivityConfiguration.Configure(modelBuilder);
            FSSCAuditorActivityConfiguration.Configure(modelBuilder);
            FSSCJobExperienceConfiguration.Configure(modelBuilder);
            FSSCAuditExperienceConfiguration.Configure(modelBuilder);
            
            // Módulo de organizaciones
            OrganizationConfiguration.Configure(modelBuilder);
            ContactConfiguration.Configure(modelBuilder);
            ShiftConfiguration.Configure(modelBuilder);
            SiteConfiguration.Configure(modelBuilder);

            // Módulo de documentación para auditorias
            AuditCycleConfiguration.Configure(modelBuilder);
            AuditCycleDocumentConfiguration.Configure(modelBuilder);
            AuditCycleStandardConfiguration.Configure(modelBuilder);
            AuditConfiguration.Configure(modelBuilder);
            AuditDocumentConfiguration.Configure(modelBuilder);
            AuditStandardConfiguration.Configure(modelBuilder);
            AuditAuditorConfiguration.Configure(modelBuilder);

            // Catálogos
            Category22KConfiguration.Configure(modelBuilder);
            NaceCodeConfiguration.Configure(modelBuilder);
            StandardConfiguration.Configure(modelBuilder);

            // Módulo de usuarios
            UserConfiguration.Configure(modelBuilder);
            RoleConfiguration.Configure(modelBuilder);
        }
    }
}