using ImportData.Entities.Databooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportData.IntegrationServicesClient.Models
{
  public class ICMDBConfigurationItemLifeCycleStates : IEntity
  {

    [PropertyOptions("Доступно для", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.Collection)]
    public IEnumerable<ICMDBConfigurationItemKinds> AvailableFor { get; set; }

    [PropertyOptions("Закрывающее", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public bool IsClosing { get; set; }

    [PropertyOptions("Примечание", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string Note { get; set; }

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

      return BusinessLogic.GetEntityWithFilter<ICMDBConfigurationItemLifeCycleStates>(x => x.Name == name, exceptionList, logger);
    }

    /// <summary>
    /// Создать состояние конфигурационной единицы
    /// </summary>
    /// <param name="propertiesForSearch">Свойства для поиска.</param>
    /// <param name="entity">Сущность.</param>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="isBatch">Признак необходимости пакетной загрузки.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Состояние конфигурационной единицы.</returns>
    new public static ICMDBConfigurationItemLifeCycleStates CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];
      var note = propertiesForSearch[Constants.KeyAttributes.Note];

      return BusinessLogic.CreateEntity<ICMDBConfigurationItemLifeCycleStates>(new ICMDBConfigurationItemLifeCycleStates()
      {
        Name = name,
        Note = note
      }, exceptionList, logger);
    }

    /// <summary>
    /// Заполнить коллекцию "Доступно для".
    /// </summary>
    /// <param name="ciKinds">Тип конфигурационной единицы.</param>
    /// <param name="logger">Логировщик.</param>
    public void AddConfigurationItemKinds(ICMDBConfigurationItemKinds ciKinds, NLog.Logger logger)
    {
      try
      {
        var result = Client.Instance().For<ICMDBConfigurationItemLifeCycleStates>()
         .Key(this)
         .NavigateTo(nameof(AvailableFor))
         .Set(new { ConfigurationItemKind = ciKinds })
         .InsertEntryAsync().Result;
      }
      catch (Exception ex)
      {
        logger.Error(ex);
        throw;
      }
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
        return BusinessLogic.CreateEntity((ICMDBConfigurationItemLifeCycleStates)entity, exceptionList, logger);
      else
      {
        return BusinessLogic.UpdateEntity((ICMDBConfigurationItemLifeCycleStates)entity, exceptionList, logger);
      }
    }
  }
}
