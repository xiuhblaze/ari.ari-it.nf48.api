using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum ProposalStatusType
    {
        Nothing,    // Nuevo registro - temporal sino se guarda, se elimina
        New,        // Nuevo registro almacenado con la información mínima
        Review,     // Registro enviado a revisión
        Rejected,   // Registro rechazado por el revisor, no se puede modificar
        Approved,   // Registro aprobado y activo
        Inactive,   // Registro inactivo, ya no está en uso, no modificable
        Cancel,     // En algún momento el registro fué cancelado
        Deleted     // Eliminación lógica
    } // ProposalStatusType
}