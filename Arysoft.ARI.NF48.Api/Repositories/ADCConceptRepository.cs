using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class ADCConceptRepository : BaseRepository<ADCConcept>
    {

        // Metodo para reordenar los conceptos de un standard por su indice,
        // acomodando el indice recibido en el orden correcto, omitiendo
        // el del ID recibido
        public void ReorderByIndex(Guid standardID, int indexSort, Guid id)
        {
            var concepts = _model
                .Where(c => c.StandardID == standardID)
                .OrderBy(c => c.IndexSort)
                .ToList();

            var index = 1;
            
            for (var i = 1; i < concepts.Count; i++) 
            {
                if (index == indexSort)
                {
                    index++;
                }

                if (concepts[i].ID != id)
                {
                    concepts[i].IndexSort = index;
                    Update(concepts[i]);
                    index++;
                }
            }            
        } // ReorderByIndex
    }
}