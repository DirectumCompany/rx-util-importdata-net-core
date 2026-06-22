using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportData.IntegrationServicesClient.Models
{
  public class ICMDBConfigurationItemRelationType : IEntity
  {
    [EntityName("Тип связи конфигурационных единиц")]

    [PropertyOptions("Группа КЕ-источника", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public ICMDBConfigurationItemCategories SourceConfigurationItemCategory { get; set; }

    [PropertyOptions("Группа КЕ-назначения", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public ICMDBConfigurationItemCategories TargetConfigurationItemCategory { get; set; }

    [PropertyOptions("Тип КЕ-источника", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public ICMDBConfigurationItemKinds SourceConfigurationItemKind { get; set; }

    [PropertyOptions("Тип КЕ-назначения", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public ICMDBConfigurationItemKinds TargetConfigurationItemKind { get; set; }

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
      var name = propertiesForSearch[Constants.KeyAttributes.Name];

      var sourceCategory = propertiesForSearch[Constants.KeyAttributes.SourceConfigurationItemCategory];
      var sourceKind = propertiesForSearch[Constants.KeyAttributes.SourceConfigurationItemKind];
      var targetCategory = propertiesForSearch[Constants.KeyAttributes.TargetConfigurationItemCategory];
      var targetKind = propertiesForSearch[Constants.KeyAttributes.TargetConfigurationItemKind];

      return BusinessLogic.GetEntityWithFilter<ICMDBConfigurationItemRelationType>(x =>
          x.Name == name &&
          x.SourceConfigurationItemCategory.Name == sourceCategory &&
          x.SourceConfigurationItemKind.Name == sourceKind &&
          x.TargetConfigurationItemCategory.Name == targetCategory &&
          x.TargetConfigurationItemKind.Name == targetKind,
          exceptionList,
          logger);
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
    new public static ICMDBConfigurationItemRelationType CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];
      var targetConfigurationItemCategoryName = propertiesForSearch[Constants.KeyAttributes.TargetConfigurationItemCategory];
      var targetConfigurationItemKindName = propertiesForSearch[Constants.KeyAttributes.TargetConfigurationItemKind];
      var sourceConfigurationItemCategoryName = propertiesForSearch[Constants.KeyAttributes.SourceConfigurationItemCategory];
      var sourceConfigurationItemKindName = propertiesForSearch[Constants.KeyAttributes.SourceConfigurationItemKind];

      var targetConfigurationItemCategory = BusinessLogic.GetEntityWithFilter<ICMDBConfigurationItemCategories>(x => x.Name == targetConfigurationItemCategoryName, exceptionList, logger);
      var targetConfigurationItemKind = BusinessLogic.GetEntityWithFilter<ICMDBConfigurationItemKinds>(x => x.Name == targetConfigurationItemKindName, exceptionList, logger);
      var sourceConfigurationItemCategory = BusinessLogic.GetEntityWithFilter<ICMDBConfigurationItemCategories>(x => x.Name == sourceConfigurationItemCategoryName, exceptionList, logger);
      var sourceConfigurationItemKind = BusinessLogic.GetEntityWithFilter<ICMDBConfigurationItemKinds>(x => x.Name == sourceConfigurationItemKindName, exceptionList, logger);

      return BusinessLogic.CreateEntity<ICMDBConfigurationItemRelationType>(new ICMDBConfigurationItemRelationType()
      {
        Name = name,
        TargetConfigurationItemCategory = targetConfigurationItemCategory,
        TargetConfigurationItemKind = targetConfigurationItemKind,
        SourceConfigurationItemCategory = sourceConfigurationItemCategory,
        SourceConfigurationItemKind = sourceConfigurationItemKind
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
        return BusinessLogic.CreateEntity((ICMDBConfigurationItemRelationType)entity, exceptionList, logger);
      else
      {
        return BusinessLogic.UpdateEntity((ICMDBConfigurationItemRelationType)entity, exceptionList, logger);
      }
    }
  }
}
