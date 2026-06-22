using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Типы запросов")]
  public class IESMRequestTypes : IEntity
  {
    [PropertyOptions("Тип запроса", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public string Name { get; set; }

    public string Status { get; set; }

    new public static IESMRequestTypes CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];

      if (string.IsNullOrWhiteSpace(name))
        return null;

      var requestType = BusinessLogic.GetEntityWithFilter<IESMRequestTypes>(x => x.Name == name, exceptionList, logger);

      if (requestType == null)
      {
        return BusinessLogic.CreateEntity<IESMRequestTypes>(new IESMRequestTypes()
        {
          Name = name,
          Status = Constants.AttributeValue[Constants.KeyAttributes.Status]
        }, exceptionList, logger);
      }

      return requestType;
    }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];

      return BusinessLogic.GetEntityWithFilter<IESMRequestTypes>(x => x.Name == name, exceptionList, logger);
    }

    public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ((IESMRequestTypes)entity).Status = Constants.AttributeValue[Constants.KeyAttributes.Status];
      if (isNewEntity)
        return BusinessLogic.CreateEntity((IESMRequestTypes)entity, exceptionList, logger);
      else
        return BusinessLogic.UpdateEntity((IESMRequestTypes)entity, exceptionList, logger);
    }
  }
}
