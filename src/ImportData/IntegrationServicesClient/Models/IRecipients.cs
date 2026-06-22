using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Субъект прав")]
  public class IRecipients : IEntity
  {
    public string Description { get; set; }
    public bool IsSystem { get; set; }
    public string Status { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
       propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.Name];

      return BusinessLogic.GetEntityWithFilter<IRecipients>(x => x.Name == name, exceptionList, logger);
    }

  }
}
