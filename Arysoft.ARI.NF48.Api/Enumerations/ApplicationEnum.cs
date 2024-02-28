namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum ApplicationStatusType
    {
        Nothing,
        New,
        Send,
        SalesReview,
        ApplicantReview,
        SalesEvaluation,
        AcceptedClient,
        RejectedClient,
        AcreditedAuditor,
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