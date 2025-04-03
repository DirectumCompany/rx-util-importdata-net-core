using System.Collections.Generic;
using System;
using System.Linq;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Журналы регистрации")]
  public class IDocumentRegisters : IEntity
  {
    [PropertyOptions("Индекс", RequiredType.Required, PropertyType.Simple)]
    public string Index { get; set; }

    [PropertyOptions("Кол-во цифр в номере", RequiredType.Required, PropertyType.Simple)]
    public int NumberOfDigitsInNumber { get; set; }

    public string ValueExample { get; set; }
    public string DisplayName { get; set; }

    [PropertyOptions("Документопоток", RequiredType.Required, PropertyType.Simple)]
    public string DocumentFlow { get; set; }

    [PropertyOptions("Тип журнала", RequiredType.Required, PropertyType.Simple)]
    public string RegisterType { get; set; }

    [PropertyOptions("Период нумерации", RequiredType.Required, PropertyType.Simple)]
    public string NumberingPeriod { get; set; }

    [PropertyOptions("Разрез нумерации", RequiredType.Required, PropertyType.Simple)]
    public string NumberingSection { get; set; }

    public string Status { get; set; }

    [PropertyOptions("Группа регистрации", RequiredType.NotRequired, PropertyType.Entity)]
    public IRegistrationGroups RegistrationGroup { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName))
      {
        if (int.TryParse(propertiesForSearch[Constants.KeyAttributes.CustomFieldName], out int documentRegisterId))
          return BusinessLogic.GetEntityWithFilter<IDocumentRegisters>(x => x.Id == documentRegisterId, exceptionList, logger);
      }
      else
      {
        var name = propertiesForSearch[Constants.KeyAttributes.Name];
        return BusinessLogic.GetEntityWithFilter<IDocumentRegisters>(x => x.Name == name, exceptionList, logger);
      }

      return null;
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        return BusinessLogic.CreateEntity((IDocumentRegisters)entity, exceptionList, logger);
      else
      {
        BusinessLogic.GetErrorResult(exceptionList, logger, Constants.Resources.EntityNotLoaded, entity.Name);
        return null;
      }
    }
  }
}
