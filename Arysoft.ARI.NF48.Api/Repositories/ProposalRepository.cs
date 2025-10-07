using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class ProposalRepository : BaseRepository<Proposal>
    {
        // Validar si una propuesta existe para un ADC

        /// <summary>
        /// Revisa si el ADC tiene una propuesta valida (no cancelada),
        /// solo puede haber una Propuesta activa por ADC
        /// </summary>
        /// <param name="adcID"></param>
        /// <returns></returns>
        public async Task<bool> ADCHasValidProposalAsync(Guid adcID)
        {
            return await _model
                .Where(p => p.ADCID == adcID
                    && p.Status > ProposalStatusType.Nothing
                    && p.Status < ProposalStatusType.Cancel)
                .AnyAsync();
        } // ADCHasValidProposalAsync

        // - Valida que el appform, auditcycle y la organization esten activos, o algo así
        // TODO: public async Task<bool> HasValidParentsAsync(Guid id)

        /// <summary>
        /// Elimina temporales creados por el usuario, funcion sobreescrita
        /// por tener un estatus diferente
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public new async Task DeleteTmpByUserAsync(string username)
        {
            foreach (var item in await _model
                .Where(m => m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                            && m.Status == ProposalStatusType.Nothing)
                .ToListAsync())
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUserAsync
    }
}