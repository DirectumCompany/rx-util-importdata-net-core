namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Регионы")]
    public class IRegions : IEntity
    {
        public string Status { get; set; }
        public string Code { get; set; }
    }
}
