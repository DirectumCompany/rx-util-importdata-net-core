using DocumentFormat.OpenXml.Wordprocessing;
using ImportData.Entities.Databooks;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Группы конфигурационных единиц")]
  public class ICMDBConfigurationItemCategories : IEntity
  {

    [PropertyOptions("Идентификатор группы конфигурационной единицы", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string ConfigurationItemCategoryGuid { get; set; }

    [PropertyOptions("Состояние", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string Status { get; set; }


    /// <summary>
    /// Создать услугу.
    /// </summary>
    /// <param name="propertiesForSearch">Свойства для поиска.</param>
    /// <param name="entity">Сущность.</param>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="isBatch">Признак необходимости пакетной загрузки.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Услуга.</returns>
    new public static ICMDBConfigurationItemCategories CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];

      return BusinessLogic.CreateEntity<ICMDBConfigurationItemCategories>(new ICMDBConfigurationItemCategories()
      {
        Name = name,
        ConfigurationItemCategoryGuid = Guid.NewGuid().ToString(),
        Status = Constants.AttributeValue[Constants.KeyAttributes.Status]
      }, exceptionList, logger);
    }

    /// <summary>
    /// Создать или изменить сущность.
    /// </summary>
    /// <param name="entity">Сущность.</param>
    /// <param name="isNewEntity">Признак новой сущности.</param>
    /// <param name="isBatch">Признак необходимости пакетной загрузки.</param>
    /// <param name="exceptionList">Список ошибок и предупреждений.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Сущность.</returns>
    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        return BusinessLogic.CreateEntity((ICMDBConfigurationItemCategories)entity, exceptionList, logger);
      else
      {
        return BusinessLogic.UpdateEntity((ICMDBConfigurationItemCategories)entity, exceptionList, logger);
      }
      
    }

    /// <summary>
    /// Найти сущность.
    /// </summary>
    /// <param name="propertiesForSearch">Свойства для поиска.</param>
    /// <param name="entity">Сущность.</param>
    /// <param name="isEntityForUpdate">Признак необходимости изменения сущности.</param>
    /// <param name="exceptionList">Список ошибок и предупреждений.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Сущность.</returns>
    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.ConfigurationItemCategory) ?
       propertiesForSearch[Constants.KeyAttributes.ConfigurationItemCategory] : propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
       propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.Name];

      return BusinessLogic.GetEntityWithFilter<ICMDBConfigurationItemCategories>(x => x.Name == name, exceptionList, logger);
    }
  }
}
