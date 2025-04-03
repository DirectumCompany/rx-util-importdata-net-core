using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Банк")]
    public  class IBanks : ICounterparties
    {
        public string TRRC { get; set; }
        public string IsCardReadOnly { get; set; }
        public string LegalName { get; set; }
        public string BIC { get; set; }
        public string CorrespondentAccount { get; set; }
        public bool IsSystem { get; set; }

        new public static IBanks FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
        {
            var name = propertiesForSearch[Constants.KeyAttributes.CustomFieldName];

            return BusinessLogic.GetEntityWithFilter<IBanks>(x => x.Name == name, exceptionList, logger);
        }
    }
}
