namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum AppFormStatusType
    {
        Nothing,            // Nuevo registro - temporal sino se guarda, se elimina
        New,                // Nuevo registro almacenado con la información mínima
        // Estos dos elementos siguientes no se van a utilizar por lo pronto (202505)
        SalesReview,        // Ventas revisa y aprueba el appForm recibido por el cliente
        SalesRejected,      // Rechazado por ventas, el cliente debe de completar más información
        // Hasta aquí
        ApplicantReview,    // Revisa que todo esté bien y es quien aprueba el appForm
        ApplicantRejected,  // Rechazado por el revisor del appForm, sales debe de completar más información
        Active,             // AppForm activo
        Inactive,           // Ya no está en uso este appForm, todo debe de estar bloqueado (solo lectura)
        Cancel,             // En algún momento el appForm fué cancelado
        Deleted             // Eliminación logica
    }

    public enum AppFormOrderType
    { 
        Nothing,
        Created,
        Organization,
        CycleYear,
        CreatedDesc,
        OrganizationDesc,
        CycleYearDesc
    }

    public enum AppFormValidityStatusType // TODO: Terminarlo, se va a ocupar luego
    { 
        Nothing,

    }
}