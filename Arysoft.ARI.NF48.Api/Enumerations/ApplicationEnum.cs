namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum ApplicationStatusType
    {
        Nothing,
        New,                // El cliente crea el app form y lo esta llenando desde su cuenta de usuario
        Send,               // El cliente envia el app form a ARI
        SalesReview,        // Ventas revisa y aprueba el app form
        ApplicantReview,    // No recuerdo quien es el applicant review pero también lo aprueba
        SalesEvaluation,    // Ventas evalua el appform y llena los dias de auditoria y eso asi como los costos, es enviado al cliente
        AcceptedClient,     // El cliente revisa y aprueba el app form
        RejectedClient,     // El cliente revisa y desaprueba el app form, se regresa a ventas
        AcreditedAuditor,   // Si se aprueba, el auditor revisa y aprueba sus días de auditoria
        // TODO: faltan estados
        Active,
        // TODO: faltan estados
        Cancel,
        Deleted
    }

    public enum ApplicationOrderType
    { 
        Nothing,
        Organization,
        Created,
        OrganizationDesc,
        CreatedDesc
    }
}