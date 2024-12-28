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

            ApplicationConfiguration.Configure(modelBuilder);
            DayCalculationConceptConfiguration.Configure(modelBuilder);
            DayCalculationConceptApplicationConfiguration.Configure(modelBuilder);
            
            AuditorConfiguration.Configure(modelBuilder);
            AuditorDocumentConfiguration.Configure(modelBuilder);
            AuditorStandardConfiguration.Configure(modelBuilder);
            CatAuditorDocumentConfiguration.Configure(modelBuilder);

            FSSCCategoryConfiguration.Configure(modelBuilder);
            FSSCSubCategoryConfiguration.Configure(modelBuilder);
            FSSCActivityConfiguration.Configure(modelBuilder);
            FSSCAuditorActivityConfiguration.Configure(modelBuilder);
            FSSCJobExperienceConfiguration.Configure(modelBuilder);
            FSSCAuditExperienceConfiguration.Configure(modelBuilder);
            
            OrganizationConfiguration.Configure(modelBuilder);
            ContactConfiguration.Configure(modelBuilder);
            ShiftConfiguration.Configure(modelBuilder);
            SiteConfiguration.Configure(modelBuilder);
            
            Category22KConfiguration.Configure(modelBuilder);
            NaceCodeConfiguration.Configure(modelBuilder);
            StandardConfiguration.Configure(modelBuilder);
            
            UserConfiguration.Configure(modelBuilder);
            RoleConfiguration.Configure(modelBuilder);
        }
    }
}