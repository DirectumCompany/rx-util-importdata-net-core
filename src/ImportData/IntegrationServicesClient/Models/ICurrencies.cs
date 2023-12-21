namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Валюта")]
    public class ICurrencies : IEntity
    {
        public string AlphaCode { get; set; }
        public string ShortName { get; set; }
        public string FractionName { get; set; }
        public bool IsDefault { get; set; }
        public string NumericCode { get; set; }
        public string Status { get; set; }
    }
}
