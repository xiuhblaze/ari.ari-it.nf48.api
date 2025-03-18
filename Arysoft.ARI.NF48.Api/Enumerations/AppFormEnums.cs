using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum AppFormStatusType
    {
        Nothing,            // Nuevo registro
        New,                // Nuevo registro almacenado con la información mínima
        Send,               // El cliente envío su información
        SalesReview,        // Ventas revisa y aprueba el appForm recibido por el cliente
        SalesRejected,      // Rechazado por ventas, el cliente debe de completar más información
        ApplicantReview,    // Revisa que todo esté bien y es quien aprueba el appForm
        ApplicantRejected,  // Rechazado por el revisor del appForm, sales debe de completar más información
        Active,             // AppForm activo
        Inactive,           // Ya no está en uso este appForm
        Cancel,             // En algún momento el appForm fué cancelado
        Deleted             // Eliminación logica
    }

    public enum AppFormOrderType
    { 
        Nothing,
        Created,
        Organization,
        CreatedDesc,
        OrganizationDesc        
    }
}