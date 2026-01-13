using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Город")]
  public class ICities : IEntity
  {
    [PropertyOptions("Населенный пункт", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public string Name { get; set; }

    public string Status { get; set; }
	
	[PropertyOptions("Регион", RequiredType.NotRequired, PropertyType.EntityWithCreate, AdditionalCharacters.ForSearch)]
    public IRegions Region { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.CustomFieldName];
      return BusinessLogic.GetEntityWithFilter<ICities>(x => x.Name == name, exceptionList, logger);
    }
	
	new public static ICities CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
      propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.Name];

      var regionName = propertiesForSearch[Constants.KeyAttributes.Region];
      var region = BusinessLogic.GetEntityWithFilter<IRegions>(x => x.Name == regionName, exceptionList, logger);

      return BusinessLogic.CreateEntity<ICities>(new ICities()
      {
        Name = name,
        Region = region,
        Status = Constants.AttributeValue[Constants.KeyAttributes.Status]
      }, exceptionList, logger);
    }
  }
}
