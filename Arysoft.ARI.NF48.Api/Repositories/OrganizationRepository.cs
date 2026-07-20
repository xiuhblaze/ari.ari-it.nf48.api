using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class OrganizationRepository : BaseRepository<Organization>
    {
        public new IEnumerable<Organization> Gets()
        { 
            return _model
                .Include(o => o.AuditCycles)
                .AsEnumerable();
        } // Gets

        public async Task<Organization> GetAsync(int folio)
        {
            return await _model
                .FirstOrDefaultAsync(o => o.Folio == folio);
        } // GetAsync

        public async Task<int> GetNextFolioAsync()
        {
            var folio = await _model
                .Where(o => 
                    o.Status > OrganizationStatusType.Applicant
                )
                .MaxAsync(o => o.Folio);

            return folio.HasValue ? folio.Value + 1 : 1;
        } // GetNextFolio

        public async Task<bool> ExistOrganizationNameAsync(string name, Guid? exceptionID)
        {
            name = name.ToLower().Trim();
            var response = _model.Where(o => o.Name.ToLower() == name);

            if (exceptionID != null && exceptionID != Guid.Empty) 
            {
                response = response.Where(o => o.ID != exceptionID);
            }

            return await response.AnyAsync();
        } // ExistOrganizationNameAsync

        /// <summary>
        /// Indica si una organización tiene más de un estándar activo asociado.
        /// </summary>
        /// <param name="id">Identificador de la organización</param>
        /// <returns>True si tiene más de un standard de lo contrario False</returns>
        /// <remarks>
        /// Autor: xBlaze
        /// Creacion: 2026-01-28
        /// Ultima Modificacion: 2026-01-28
        /// </remarks>
        public async Task<bool> IsMultiStandardAsync(Guid id)
        {
            var organization = await _model
                .Include(o => o.OrganizationStandards)
                .FirstOrDefaultAsync(o => o.ID == id)
                ?? throw new BusinessException("The organization not found");
            var activeStandardsCount = organization.OrganizationStandards
                .Count(s => s.Status == StatusType.Active);

            return activeStandardsCount > 1;
        } // IsMultiStandard

        public new async Task DeleteTmpByUserAsync(string username)
        {
            var items = await _model
                .Include(o => o.AuditCycles)
                .Include(o => o.Companies)
                .Include(o => o.Contacts)
                .Include(o => o.Sites.Select(s => s.Shifts))
                //.Include(o => o.Certificates)
                .Where(m =>
                    m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                    && m.Status == OrganizationStatusType.Nothing
                ).ToListAsync();

            foreach (var item in items)
            {   
                _model.Remove(item);
            }
        } // DeleteTmpByUser
    }
}