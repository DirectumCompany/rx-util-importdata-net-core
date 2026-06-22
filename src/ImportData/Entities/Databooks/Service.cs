using System;
using System.Collections.Generic;
using System.Linq;
using ImportData.IntegrationServicesClient.Models;
using NLog;

namespace ImportData.Entities.Databooks
{
  class Service : Entity
  {
    /// <summary>
    /// Количество свойств.
    /// </summary>
    public override int PropertiesCount { get { return 11; } }
    
    /// <summary>
    /// Тип сущности.
    /// </summary>
    protected override Type EntityType { get { return typeof(IESMServices); } }

    /// <summary>
    /// Заполнить значения свойств.
    /// </summary>
    /// <param name="exceptionList">Список ошибок и предупреждений.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Признак наличия ошибок при заполнении значений свойств.</returns>
    protected override bool FillProperties(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];
      return false;
    }

    /// <summary>
    /// Обработать событие "После сохранения" сущности.
    /// </summary>
    /// <param name="logger">Логировщик.</param>
    /// <param name="ignoreDuplicates">Признак необходимости игнорировать дубликаты.</param>
    /// <param name="isBatch">Признак необходимости пакетной загрузки.</param>
    /// <returns>Список ошибок и предупреждений.</returns>
    public override IEnumerable<Structures.ExceptionsStruct> SaveToRX(Logger logger, string ignoreDuplicates, bool isBatch = false)
    {
      var exceptionList = new List<Structures.ExceptionsStruct>();

      exceptionList.AddRange(base.SaveToRX(logger, ignoreDuplicates, isBatch));

      // Добавление логотипа услуги.
      if (NamingParameters.ContainsKey(Constants.KeyAttributes.LogoRu) && isNewEntity)
      {
        var filePath = NamingParameters[Constants.KeyAttributes.LogoRu];

        if (!string.IsNullOrWhiteSpace(filePath) && entity != null)
          exceptionList.AddRange(BusinessLogic.AddImage((IEntityWithImage)entity, filePath, logger));
      }

      return exceptionList;
    }

    /// <summary>
    /// Заполнить свойства коллекции.
    /// </summary>
    /// <param name="exceptionList">Список ошибок и предупреждений.</param>
    /// <param name="logger">Логировщик.</param>
    protected override void FillCollections(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      // Импорт категорий услуг.
      var service = entity as IESMServices;
      var serviceCategories = ((string)ResultValues[Constants.KeyAttributes.CategoriesCollection]).Split(";");
      foreach (var serviceCategory in serviceCategories)
      {
        var serviceCategoryEntity = BusinessLogic.GetEntityWithFilter<IESMServiceCategories>(x => x.Name == serviceCategory.Trim(), exceptionList, logger);
        if (serviceCategoryEntity != null)
          service.AddServiceCategory(serviceCategoryEntity, logger);
      }

      // Импорт субъектов прав для кого доступна услуга.
      var recipients = ((string)ResultValues[Constants.KeyAttributes.AvailableCollection]).Split(";");
      foreach (var recipient in recipients)
      {
        var recipientEntity = BusinessLogic.GetEntityWithFilter<IRecipients>(x => x.Name == recipient.Trim(), exceptionList, logger);
        if (recipientEntity != null)
          service.AddAvailableFor(recipientEntity, logger);
      }

      // Импорт запросов к инструменту ИИ.
      var intents = ((string)ResultValues[Constants.KeyAttributes.IntentExamplesForTool]).Split(";", StringSplitOptions.RemoveEmptyEntries);
      foreach (var intent in intents.ToHashSet())
      {
        var intentString = intent.Trim();
        if (!string.IsNullOrEmpty(intentString))
        {
          service.AddIntent(intentString, logger);
        }
      }
    }
  }
}
