using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Валюта")]
  public class ICurrencies : IEntity
  {
    [PropertyOptions("Буквенный код", RequiredType.Required, PropertyType.Simple)]
    public string AlphaCode { get; set; }

    [PropertyOptions("Сокр. Наименование", RequiredType.Required, PropertyType.Simple)]
    public string ShortName { get; set; }

    [PropertyOptions("Дробная часть", RequiredType.Required, PropertyType.Simple)]
    public string FractionName { get; set; }

    [PropertyOptions("", RequiredType.NotRequired, PropertyType.Simple)]
    public bool IsDefault { get; set; }

    [PropertyOptions("Цифровой код", RequiredType.Required, PropertyType.Simple)]
    public string NumericCode { get; set; }

    [PropertyOptions("Состояние", RequiredType.Required, PropertyType.Simple)]
    public string Status { get; set; }

    new public static ICurrencies FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
        propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.Name];

      return BusinessLogic.GetEntityWithFilter<ICurrencies>(x => x.Name == name, exceptionList, logger);
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        return BusinessLogic.CreateEntity((ICurrencies)entity, exceptionList, logger);
      else
        return BusinessLogic.UpdateEntity((ICurrencies)entity, exceptionList, logger);
    }
  }
}
