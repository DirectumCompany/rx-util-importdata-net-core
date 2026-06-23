using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Срочности")]
  public class IESMUrgencies : IEntity
  {
    public bool? IsDefault { get; set; }

    public int TimeThresholdPercent { get; set; }

    [PropertyOptions("Описание", RequiredType.NotRequired, PropertyType.Simple)]
    public string Description { get; set; }

    public string Status { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
       propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.Name];

      return BusinessLogic.GetEntityWithFilter<IESMUrgencies>(x => x.Name == name, exceptionList, logger);
    }
  }
}
