using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Уровни влияния")]
  public class IESMInfluences : IEntity
  {
    [PropertyOptions("Состояние", RequiredType.Required, PropertyType.Simple)]
    public string Status { get; set; }

    [PropertyOptions("Описание", RequiredType.NotRequired, PropertyType.Simple)]
    public string Description { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
       propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.Name];

      return BusinessLogic.GetEntityWithFilter<IESMInfluences>(x => x.Name == name, exceptionList, logger);
    }
  }
}
