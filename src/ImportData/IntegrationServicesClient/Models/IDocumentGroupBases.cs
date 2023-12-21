namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Группа документов")]
    public class IDocumentGroupBases : IEntity
    {
        public string Note { get; set; }
        public string Status { get; set; }
    }
}
