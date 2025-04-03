using ImportData.Entities.Databooks;
using System;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Виды документов")]
  public class IDocumentKinds : IEntity
  {
    [PropertyOptions("Примечание", RequiredType.NotRequired, PropertyType.Simple)]
    public string Note { get; set; }

    [PropertyOptions("Рассмотрение дней", RequiredType.NotRequired, PropertyType.Simple)]
    public int? DeadlineInDays { get; set; }

    [PropertyOptions("Сокращенное имя", RequiredType.Required, PropertyType.Simple)]
    public string ShortName { get; set; }

    [PropertyOptions("часов", RequiredType.NotRequired, PropertyType.Simple)]
    public int? DeadlineInHours { get; set; }

    public bool GenerateDocumentName { get; set; }

    public bool AutoNumbering { get; set; }

    public bool ProjectsAccounting { get; set; }

    public bool GrantRightsToProject { get; set; }

    public bool IsDefault { get; set; }

    [PropertyOptions("Код", RequiredType.NotRequired, PropertyType.Simple)]
    public string Code { get; set; }

    [PropertyOptions("Документопоток", RequiredType.Required, PropertyType.Simple)]
    public string DocumentFlow { get; set; }

    [PropertyOptions("Тип нумерации", RequiredType.Required, PropertyType.Simple)]
    public string NumberingType { get; set; }

    public string Status { get; set; }

    [PropertyOptions("Тип документа", RequiredType.Required, PropertyType.Entity)]
    public IDocumentType DocumentType { get; set; }

    // Поле заполняется при создании дефолтными значениями.
    public IEnumerable<IEntityBase> AvailableActions { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var kind = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
        propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.Name];

      return BusinessLogic.GetEntityWithFilter<IDocumentKinds>(x => x.Name == kind, exceptionList, logger);
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        return BusinessLogic.CreateEntity((IDocumentKinds)entity, exceptionList, logger);
      else
      {
        BusinessLogic.GetErrorResult(exceptionList, logger, Constants.Resources.EntityNotLoaded, entity.Name);
        return null;
      }
    }
  }
}
