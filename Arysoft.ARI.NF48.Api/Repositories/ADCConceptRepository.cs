using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class ADCConceptRepository : BaseRepository<ADCConcept>
    {
        /// <summary>
        /// Metodo para reordenar los conceptos de un standard por su indice,
        /// acomodando el indice recibido en el orden correcto, omitiendo
        /// el del ID recibido
        /// </summary>
        /// <param name="standardID">ID del estandar para obtener sus Conceptos solamente</param>
        /// <param name="indexSort">Posicion en que se quiere colocar el Concepto</param>
        /// <param name="id">ID del Concepto a reubicar y reordenar todo</param>
        public void ReorderByIndex(Guid standardID, int indexSort, Guid id)
        {
            var concepts = _model
                .Where(c => c.StandardID == standardID 
                    && c.Status == StatusType.Active)
                .OrderBy(c => c.IndexSort)
                .ToList();

            var index = 1;

            foreach (var concept in concepts)
            { 
                if (index == indexSort) index++;

                if (concept.ID != id)
                {
                    concept.IndexSort = index;
                    Update(concept);
                    index++;
                }
            }
        } // ReorderByIndex
    }
}