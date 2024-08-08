using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Страны")]
  public class ICountries : IEntity
  {
    [PropertyOptions("Состояние", RequiredType.Required, PropertyType.Simple)]
    public string Status { get; set; }

    [PropertyOptions("Код", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string Code { get; set; }

    new public static ICountries FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];
      var code = propertiesForSearch[Constants.KeyAttributes.Code];

      return BusinessLogic.GetEntityWithFilter<ICountries>(x => x.Name == name && x.Code == code, exceptionList, logger);
    }

    new public static void CreateOrUpdate(IEntity entity, bool isNewEntity, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        BusinessLogic.CreateEntity((ICountries)entity, exceptionList, logger);
      else
        BusinessLogic.UpdateEntity((ICountries)entity, exceptionList, logger);
    }
  }
}
