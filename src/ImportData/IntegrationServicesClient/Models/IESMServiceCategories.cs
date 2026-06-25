using ImportData.Entities.Databooks;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Категории услуг")]
  public class IESMServiceCategories : IEntity, IEntityWithImage
  {
    [PropertyOptions("Родительская категория", RequiredType.NotRequired, PropertyType.EntityWithCreate)]
    public IESMServiceCategories SubsidiaryCategory { get; set; }

    /// <summary>
    /// Логотип.
    /// </summary>
    public IBinaryData Logo { get; set; }

    [PropertyOptions("Описание", RequiredType.NotRequired, PropertyType.Simple)]
    public string Description { get; set; }

    /// <summary>
    /// Состояние.
    /// </summary>
    public string Status { get; set; }

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

      return BusinessLogic.GetEntityWithFilter<IESMServiceCategories>(x => x.Name == name, exceptionList, logger);
    }

    /// <summary>
    /// Создать категорию услуг.
    /// </summary>
    /// <param name="propertiesForSearch">Свойства для поиска.</param>
    /// <param name="entity">Сущность.</param>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="isBatch">Признак необходимости пакетной загрузки.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Категория услуг.</returns>
    new public static IESMServiceCategories CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];
      var subsidiaryCategoryName = propertiesForSearch[Constants.KeyAttributes.SubsidiaryCategory];
      var subsidiaryCategory = BusinessLogic
        .GetEntityWithFilter<IESMServiceCategories>(x => x.Name == subsidiaryCategoryName, exceptionList, logger);

      return BusinessLogic.CreateEntity<IESMServiceCategories>(new IESMServiceCategories()
      {
        Name = name,
        SubsidiaryCategory = subsidiaryCategory,
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
        return BusinessLogic.CreateEntity((IESMServiceCategories)entity, exceptionList, logger);
      else
      {
        return BusinessLogic.UpdateEntity((IESMServiceCategories)entity, exceptionList, logger);
      }
    }

    /// <summary>
    /// Добавить картинку для сущности.
    /// </summary>
    /// <param name="image">Картинка.</param>
    /// <returns>Результат добавления картинки.</returns>
    public bool AddImage(IBinaryData image)
    {
      byte[] encodingContent = image.Value;
      return Client.Instance().For<IESMServiceCategories>()
      .Key(this)
      .NavigateTo(x => x.Logo)
      .Set(new { Value = encodingContent })
      .InsertEntryAsync()
      .Result != null;
    }
  }
}
