using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Должность")]
  public class IJobTitles : IEntity
  {
    [PropertyOptions("Должность", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public string Name { get; set; }

    public string Status { get; set; }

    new public static IJobTitles CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];

      if (string.IsNullOrWhiteSpace(name))
        return null;

      var jobTitle = BusinessLogic.GetEntityWithFilter<IJobTitles>(x => x.Name == name, exceptionList, logger);

      if (jobTitle == null)
      {
        return BusinessLogic.CreateEntity<IJobTitles>(new IJobTitles()
        {
          Name = name,
          Status = Constants.AttributeValue[Constants.KeyAttributes.Status]
        }, exceptionList, logger);
      }

      return jobTitle;
    }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];

      return BusinessLogic.GetEntityWithFilter<IJobTitles>(x => x.Name == name, exceptionList, logger);
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ((IJobTitles)entity).Status = Constants.AttributeValue[Constants.KeyAttributes.Status];
      if (isNewEntity)
        return BusinessLogic.CreateEntity((IJobTitles)entity, exceptionList, logger);
      else
        return BusinessLogic.UpdateEntity((IJobTitles)entity, exceptionList, logger);
    }
  }
}
