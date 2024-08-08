using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Регионы")]
  public class IRegions : IEntity
  {
    [PropertyOptions("Регион", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public string Name { get; set; }

    [PropertyOptions("Состояние", RequiredType.Required, PropertyType.Simple)]
    public string Status { get; set; }

    [PropertyOptions("Код", RequiredType.Required, PropertyType.Simple)]
    public string Code { get; set; }

    new public static IRegions FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.CustomFieldName];

      return BusinessLogic.GetEntityWithFilter<IRegions>(x => x.Name == name, exceptionList, logger);
    }
  }
}
