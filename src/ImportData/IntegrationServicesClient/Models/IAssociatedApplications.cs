using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Приложение-обработчик")]
  public class IAssociatedApplications : IEntity
  {
    [PropertyOptions("Расширение", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string Extension { get; set; }

    [PropertyOptions("Тип отслеживания закрытия", RequiredType.Required, PropertyType.Simple)]
    public string MonitoringType { get; set; }

    [PropertyOptions("По умолчанию открывать для чтения", RequiredType.NotRequired, PropertyType.Simple)]
    public bool OpenByDefaultForReading { get; set; }

    public string Status { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var extension = propertiesForSearch[Constants.KeyAttributes.Extension];
      return BusinessLogic.GetEntityWithFilter<IAssociatedApplications>(x => x.Extension == extension, exceptionList, logger);
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        return BusinessLogic.CreateEntity((IAssociatedApplications)entity, exceptionList, logger);
      else
      {
        BusinessLogic.GetErrorResult(exceptionList, logger, Constants.Resources.EntityNotLoaded, entity.Name);
        return null;
      }
    }

  }
}
