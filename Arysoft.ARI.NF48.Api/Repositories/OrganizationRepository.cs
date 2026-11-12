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
        //public new IEnumerable<Organization> Gets()
        //{ 
        //    return _model
        //        .Include(o => o.AuditCycles)
        //        .AsEnumerable();
        //} // Gets

        /// <summary>
        /// Detalle de la organización. Una sola colección va en el Include (con su
        /// referencia Standard, evita el N+1) y las demás se cargan con un query
        /// pequeño cada una, así no se multiplican las filas del resultado.
        /// </summary>
        public override async Task<Organization> GetAsync(Guid id, bool asNoTracking = false)
        {
            var query = _model
                .Include(o => o.OrganizationStandards.Select(os => os.Standard));

            // Sin tracking no se puede usar Entry().Collection(): solo viaja el Include
            // anterior y el resto de colecciones queda en null (el lazy loading tampoco
            // funciona en entidades sin tracking).
            if (asNoTracking)
                return await query.AsNoTracking().FirstOrDefaultAsync(o => o.ID == id);

            var item = await query.FirstOrDefaultAsync(o => o.ID == id);
            if (item == null) return null;

            var entry = _context.Entry(item);
            await entry.Collection(o => o.AuditCycles).LoadAsync();
            await entry.Collection(o => o.Companies).LoadAsync();
            await entry.Collection(o => o.Contacts).LoadAsync();
            await entry.Collection(o => o.Notes).LoadAsync();
            await entry.Collection(o => o.Sites).LoadAsync();

            return item;
        } // GetAsync

        /// <summary>
        /// Para el LISTADO: carga las relaciones de TODA la página con un query por
        /// colección (6 queries en total, sin importar el tamaño de la página), en lugar
        /// de uno por organización (lazy loading) o un JOIN gigante (Include).
        /// Requiere que las organizaciones estén rastreadas (sin AsNoTracking): EF une
        /// cada fila hija con su Organization por relationship fixup.
        /// Se asume que cada hijo tiene la FK OrganizationID.
        /// </summary>
        public async Task LoadListRelationsAsync(IList<Organization> organizations)
        {
            if (organizations == null || organizations.Count == 0) return;

            var ids = organizations.Select(o => o.ID).ToList();

            await _context.Set<AuditCycle>().Where(x => ids.Contains(x.OrganizationID)).LoadAsync();
            await _context.Set<Company>().Where(x => ids.Contains(x.OrganizationID)).LoadAsync();
            await _context.Set<Contact>().Where(x => ids.Contains(x.OrganizationID)).LoadAsync();
            await _context.Set<Note>().Where(x => ids.Contains(x.OwnerID)).LoadAsync();
            await _context.Set<Site>().Where(x => ids.Contains(x.OrganizationID)).LoadAsync();
            await _context.Set<OrganizationStandard>()
                .Where(x => ids.Contains(x.OrganizationID))
                .Include(x => x.Standard)
                .LoadAsync();

            // Sin esto, el proxy de lazy loading vuelve a consultar la BD la primera vez
            // que se lee cada colección, aunque ya esté poblada por el fixup.
            foreach (var organization in organizations)
            {
                var entry = _context.Entry(organization);
                entry.Collection(o => o.AuditCycles).IsLoaded = true;
                entry.Collection(o => o.Companies).IsLoaded = true;
                entry.Collection(o => o.Contacts).IsLoaded = true;
                entry.Collection(o => o.Notes).IsLoaded = true;
                entry.Collection(o => o.Sites).IsLoaded = true;
                entry.Collection(o => o.OrganizationStandards).IsLoaded = true;
            }
        } // LoadListRelationsAsync

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