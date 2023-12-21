namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Внутренний документ")]
    public class IInternalDocumentBases : IOfficialDocuments
    {
        public IEmployees Assignee { get; set; }
        public IBusinessUnits BusinessUnit { get; set; }
    }
}
