using DocumentFormat.OpenXml.Wordprocessing;
using ImportData.Entities.Databooks;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Услуги")]
  public class IESMServices : IEntity, IEntityWithImage
  {
    [PropertyOptions("Менеджер услуги", RequiredType.NotRequired, PropertyType.Entity)]
    public IEmployees ServiceManager { get; set; }

    [PropertyOptions("Тип запроса", RequiredType.Required, PropertyType.EntityWithCreate)]
    public IESMRequestTypes RequestType { get; set; }

    [PropertyOptions("Группа конфигурационной единицы", RequiredType.NotRequired, PropertyType.Entity)]
    public ICMDBConfigurationItemCategories ConfigurationUnitCategory { get; set; }

    [PropertyOptions("Срочность", RequiredType.Required, PropertyType.Entity)]
    public IESMUrgencies Urgency { get; set; }

    [PropertyOptions("Влияние", RequiredType.NotRequired, PropertyType.Entity)]
    public IESMInfluences Influence { get; set; }

    [PropertyOptions("Категории услуг", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.Collection)]
    public IEnumerable<IESMServiceCategories> CategoriesCollection { get; set; }

    [PropertyOptions("Доступны для", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.Collection)]
    public IEnumerable<IRecipients> AvailableCollection { get; set; }

    [PropertyOptions("Описание", RequiredType.NotRequired, PropertyType.Simple)]
    public string Description { get; set; }

    [PropertyOptions("Примеры запросов пользователя", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.Collection)]
    public IEnumerable<string> IntentExamplesForTool { get; set; }

    /// <summary>
    /// Логотип.
    /// </summary>
    public IBinaryData ServiceLogo { get; set; }

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

      return BusinessLogic.GetEntityWithFilter<IESMServices>(x => x.Name == name, exceptionList, logger);
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
    new public static IESMServices CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];
      var requestTypeName = propertiesForSearch[Constants.KeyAttributes.RequestType];
      var requestType = BusinessLogic.GetEntityWithFilter<IESMRequestTypes>(x => x.Name == requestTypeName, exceptionList, logger);
      var urgencyName = propertiesForSearch[Constants.KeyAttributes.Urgency];
      var urgency = BusinessLogic.GetEntityWithFilter<IESMUrgencies>(x => x.Name == urgencyName, exceptionList, logger);

      return BusinessLogic.CreateEntity<IESMServices>(new IESMServices()
      {
        Name = name,
        RequestType = requestType,
        Urgency = urgency,
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
        return BusinessLogic.CreateEntity((IESMServices)entity, exceptionList, logger);
      else
      {
        return BusinessLogic.UpdateEntity((IESMServices)entity, exceptionList, logger);
      }
    }

    /// <summary>
    /// Добавить категорию услуги в карточку услуги.
    /// </summary>
    /// <param name="serviceCategory">Категория услуги.</param>
    /// <param name="logger">Логировщик.</param>
    public void AddServiceCategory(IESMServiceCategories serviceCategory, NLog.Logger logger)
    {
      try
      {
        var result = Client.Instance().For<IESMServices>()
         .Key(this)
         .NavigateTo(nameof(CategoriesCollection))
         .Set(new { CategoriesServices = serviceCategory })
         .InsertEntryAsync().Result;
      }
      catch (Exception ex)
      {
        logger.Error(ex);
        throw;
      }
    }

    /// <summary>
    /// Добавить субъект прав, для которого доступна услуга.
    /// </summary>
    /// <param name="recipient">Субъект прав.</param>
    /// <param name="logger">Логировщик.</param>
    public void AddAvailableFor(IRecipients recipient, NLog.Logger logger)
    {
      try
      {
        var result = Client.Instance().For<IESMServices>()
         .Key(this)
         .NavigateTo(nameof(AvailableCollection))
         .Set(new { AvailableFor = recipient })
         .InsertEntryAsync().Result;
      }
      catch (Exception ex)
      {
        logger.Error(ex);
        throw;
      }
    }

    /// <summary>
    /// Добавить пример запроса к инструменту ИИ.
    /// </summary>
    /// <param name="intent">Пример запроса.</param>
    /// <param name="logger">Логировщик.</param>
    public void AddIntent(string intent, NLog.Logger logger)
    {
      try
      {
        var result = Client.Instance().For<IESMServices>()
         .Key(this)
         .NavigateTo(nameof(IntentExamplesForTool))
         .Set(new { Example = intent })
         .InsertEntryAsync().Result;
      }
      catch (Exception ex)
      {
        logger.Error($"{intent}:{ex}");
        throw;
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
      return Client.Instance().For<IESMServices>()
      .Key(this)
      .NavigateTo(x => x.ServiceLogo)
      .Set(new { Value = encodingContent })
      .InsertEntryAsync()
      .Result != null;
    }
  }
}
