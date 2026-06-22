using DocumentFormat.OpenXml.Bibliography;
using ImportData.IntegrationServicesClient;
using ImportData.IntegrationServicesClient.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImportData.Entities.Databooks
{
  class ServiceCategory : Entity
  {
    /// <summary>
    /// Количество свойств.
    /// </summary>
    public override int PropertiesCount { get { return 4; } }

    /// <summary>
    /// Тип сущности.
    /// </summary>
    protected override Type EntityType { get { return typeof(IESMServiceCategories); } }

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

      // Добавление логотипа категории услуг.
      if (NamingParameters.ContainsKey(Constants.KeyAttributes.LogoRu) && isNewEntity)
      {
        var filePath = NamingParameters[Constants.KeyAttributes.LogoRu];

        if (!string.IsNullOrWhiteSpace(filePath) && entity != null)
          exceptionList.AddRange(BusinessLogic.AddImage((IEntityWithImage)entity, filePath, logger));
      }

      return exceptionList;
    }
  }
}
