using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Срок хранения дела")]
  public class IFileRetentionPeriods : IEntity
  {
    [PropertyOptions("Наименование срока хранения", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public string Name { get; set; }

    [PropertyOptions("Срок хранения", RequiredType.NotRequired, PropertyType.Simple)]
    public int? RetentionPeriod { get; set; }

    public string Note { get; set; }
    public string Status { get; set; }

    new public static IEntity CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];

      return BusinessLogic.CreateEntity(new IFileRetentionPeriods()
      {
        Name = name,
        Status = Constants.AttributeValue[Constants.KeyAttributes.Status]
      }, exceptionList, logger);
    }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];

      return BusinessLogic.GetEntityWithFilter<IFileRetentionPeriods>(x => x.Name == name, exceptionList, logger);
    }

    new public static void CreateOrUpdate(IEntity entity, bool isNewEntity, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ((IFileRetentionPeriods)entity).Status = Constants.AttributeValue[Constants.KeyAttributes.Status];
      if (isNewEntity)
        BusinessLogic.CreateEntity((IFileRetentionPeriods)entity, exceptionList, logger);
      else
        BusinessLogic.UpdateEntity((IFileRetentionPeriods)entity, exceptionList, logger);
    }
  }
}
