namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum ProposalStatusType
    {
        Nothing,    // Nuevo registro - temporal sino se guarda, se elimina
        New,        // Nuevo registro almacenado con la información mínima
        Review,     // Registro enviado a revisión
        Rejected,   // Registro rechazado por el revisor o el cliente, no se puede modificar
        Approved,   // Registro aprobado y listo para enviar al cliente para firma
        Sended,     // Registro enviado al cliente para firma - El sistema no lo envía
        Active,     // Registro firmado por el cliente y activo - Registro manual
        Inactive,   // Registro inactivo, ya no está en uso, no modificable - Generar historial
        Cancel,     // En algún momento el registro fué cancelado
        Deleted     // Eliminación lógica
    } // ProposalStatusType

    public enum ProposalOrderType
    {
        Nothing,
        Created,
        Status,
        CreatedDesc,
        StatusDesc,
    } // ProposalOrderType

    public enum ProposalAlertType
    { 
        Nothing,
        ScopeMistmatch,
        EmployeesMistmatch,
    }
}