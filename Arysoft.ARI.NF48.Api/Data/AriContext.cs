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
                        
            // Módulo de auditores 
            AuditorConfiguration.Configure(modelBuilder);
            AuditorDocumentConfiguration.Configure(modelBuilder);
            AuditorStandardConfiguration.Configure(modelBuilder);
            CatAuditorDocumentConfiguration.Configure(modelBuilder);

            // Módulo de organizaciones
            OrganizationConfiguration.Configure(modelBuilder);
            OrganizationStandardConfiguration.Configure(modelBuilder);
            CertificateConfiguration.Configure(modelBuilder);
            CompanyConfiguration.Configure(modelBuilder);
            ContactConfiguration.Configure(modelBuilder);
            ShiftConfiguration.Configure(modelBuilder);
            SiteConfiguration.Configure(modelBuilder);

            AppFormConfiguration.Configure(modelBuilder);

            // Módulo de ADC - Audit Day Calculation
            ADCConfiguration.Configure(modelBuilder);
            ADCConceptConfiguration.Configure(modelBuilder);
            ADCConceptValueConfiguration.Configure(modelBuilder);
            ADCSiteConfiguration.Configure(modelBuilder);
            MD5Configuration.Configure(modelBuilder);
            ADCSiteAuditConfiguration.Configure(modelBuilder);

            // Módulo de Auditorias
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
            NoteConfiguration.Configure(modelBuilder);
            StandardConfiguration.Configure(modelBuilder);

            // Módulo de usuarios
            UserConfiguration.Configure(modelBuilder);
            RoleConfiguration.Configure(modelBuilder);
            UserSettingConfiguration.Configure(modelBuilder);
        }
    }
}