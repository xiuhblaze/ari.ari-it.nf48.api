namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum ADCSiteOrderType
    {
        Nothing,
        SiteDescription,
        IsMainSite,
        SiteDescriptionDesc,
        IsMainSiteDesc,
    }

    public enum ADCSiteAlertType
    { 
        Nothing,            // No hay alerta
        EmployeesMistmatch, // Alerta de discrepancia en el número de empleados
    }
}