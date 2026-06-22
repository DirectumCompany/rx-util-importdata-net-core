using ImportData.Entities.Databooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Тип конфигурационной единицы")]
  public class ICMDBConfigurationItemKinds : IEntity
  {
    [PropertyOptions("Описание", RequiredType.NotRequired, PropertyType.Simple)]
    public string Description { get; set; }

    public string Status { get; set; }

    [PropertyOptions("Ответственный за вид КЕ", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IEmployees Responsible { get; set; }

    [PropertyOptions("Группа КЕ", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public ICMDBConfigurationItemCategories ConfigurationItemCategory { get; set; }

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
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
      propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.Name];

      return BusinessLogic.GetEntityWithFilter<ICMDBConfigurationItemKinds>(x => x.Name == name, exceptionList, logger);
    }

    /// <summary>
    /// Создать услугу.
    /// </summary>
    /// <param name="propertiesForSearch">Свойства для поиска.</param>
    /// <param name="entity">Сущность.</param>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="isBatch">Признак необходимости пакетной загрузки.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Услуга.</returns>
    new public static ICMDBConfigurationItemKinds CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];
      var description = propertiesForSearch["Description"];
      var responsibleName = propertiesForSearch[Constants.KeyAttributes.Responsible];
      var responsible = BusinessLogic.GetEntityWithFilter<IEmployees>(x => x.Name == responsibleName, exceptionList, logger);
      var configurationItemCategoryName = propertiesForSearch[Constants.KeyAttributes.ConfigurationItemCategory];
      var configurationItemCategory = BusinessLogic.GetEntityWithFilter<ICMDBConfigurationItemCategories>(x => x.Name == configurationItemCategoryName, exceptionList, logger);

      return BusinessLogic.CreateEntity<ICMDBConfigurationItemKinds>(new ICMDBConfigurationItemKinds()
      {
        Name = name,
        Description = description,
        Responsible = responsible,
        ConfigurationItemCategory = configurationItemCategory,
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
        return BusinessLogic.CreateEntity((ICMDBConfigurationItemKinds)entity, exceptionList, logger);
      else
      {
        return BusinessLogic.UpdateEntity((ICMDBConfigurationItemKinds)entity, exceptionList, logger);
      }
    }
  }
}
