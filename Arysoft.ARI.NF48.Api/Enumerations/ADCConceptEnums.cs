namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum ADCConceptUnitType
    {
        Nothing,        // No se ha definido unidad        
        Percentage,     // Porcentaje        
        Days            // Días        
    }

    public enum ADCConceptCustomFunctionType
    {
        Nothing,            // No se ha definido función        
        ForAdditionalSite,  // Para sitio adicional
    }

    public enum ADCConceptOrderType
    { 
        Nothing,
        IndexSort,
        Description,
        Standard,
        IndexSortDesc,
        DescriptionDesc,
        StandardDesc
    }
}