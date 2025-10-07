using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class StandardRepository : BaseRepository<Standard>
    {
        public async Task<bool> ExistNameAsync(string name, Guid exceptionID)
        {
            return await _model
                .Where(m => 
                    m.Name.ToUpper() == name.ToUpper().Trim()
                    && m.ID != exceptionID
                ).AnyAsync();
        } // ExistNameAsync

        //public override async Task DeleteTmpByUser(string username)
        //{
        //    var items = await _model
        //        .Where(m =>
        //            m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
        //            && m.Status == StatusType.Nothing
        //        ).ToListAsync();

        //    foreach (var item in items)
        //    {
        //        _model.Remove(item);
        //    }
        //} // DeleteTmpByUser

        /// <summary>
        /// Verifica si el estandar tiene alguna asociacion
        /// (Organizaciones, Auditores, AppForms, AuditCycles, Audits,
        /// Documents, RiskLevels, Auditor documents, etc)
        /// - Creo que con los básicos es suficiente
        /// </summary>
        /// <param name="standardID">ID del standard a validar</param>
        /// <returns></returns>
        public async Task<bool> IsAnyAssociationAsync(Guid standardID)
        {
            if (await _context.Set<StandardTemplate>()
                .Where(st => st.StandardID == standardID
                    && st.Status != StatusType.Nothing)
                .AnyAsync()) return true;

            if (await _context.Set<OrganizationStandard>()
                .Where(st => st.StandardID == standardID
                    && st.Status != StatusType.Nothing)
                .AnyAsync()) return true;

            if (await _context.Set<AuditorStandard>()
                .Where(st => st.StandardID == standardID
                    && st.Status != StatusType.Nothing)
                .AnyAsync()) return true;

            if (await _context.Set<AuditStandard>()
                .Where(st => st.StandardID == standardID
                    && st.Status != StatusType.Nothing)
                .AnyAsync()) return true;

            return false;
        } // IsAnyAssociationAsync
    }
}