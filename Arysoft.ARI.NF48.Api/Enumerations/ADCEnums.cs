namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum ADCStatusType
    {
        Nothing,    // Nuevo registro - temporal sino se guarda, se elimina
        New,        // Nuevo registro almacenado con la información mínima
        Review,     // Registro enviado a revisión
        Active,     // Registro aprobado y activo
        Inactive,   // Registro inactivo, ya no está en uso, no modificable
        Cancel,     // En algún momento el registro fué cancelado
        Deleted     // Eliminación lógica
    }
}