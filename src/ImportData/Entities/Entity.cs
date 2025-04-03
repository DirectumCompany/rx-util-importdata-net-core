using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Word;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using ImportData.IntegrationServicesClient;
using ImportData.IntegrationServicesClient.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ImportData
{
  public class Entity
  {
    public Dictionary<string, string> NamingParameters { get; set; }
    public Dictionary<string, string> ExtraParameters { get; set; }
    public Dictionary<string, object> ResultValues { get; set; }
    protected virtual Type EntityType { get; }
    protected IEntityBase entity = null;
    protected bool isNewEntity = false;
    public virtual int RequestsPerBatch { get; } = 0;

    /// <summary>
    /// Количество используемых параметров.
    /// </summary>
    public virtual int PropertiesCount { get; }

    /// <summary>
    /// Сохранение сущности в RX.
    /// </summary>
    /// <param name="logger">Логировщик.</param>
    /// <param name="ignoreDuplicates">Игнорирование дублей.</param>
    /// <param name="isBatch">Пакетный импорт.</param>
    /// <returns>Список ошибок.</returns>
    public virtual IEnumerable<Structures.ExceptionsStruct> SaveToRX(NLog.Logger logger, string ignoreDuplicates, bool isBatch = false)
    {
      var exceptionList = new List<Structures.ExceptionsStruct>();
      ResultValues = new Dictionary<string, object>();

      var properties = EntityType.GetProperties();
      foreach (var property in properties)
      {
        var options = BusinessLogic.GetPropertyOptions(property);
        if (options == null)
          continue;

        object variableForParameters = null;
        // Обработка свойств модели, которые заполняются/создаются из нескольких столбцов шаблона.
        if (options.Characters == AdditionalCharacters.CreateFromOtherProperties)
        {
          var propertiesForSearch = GetPropertiesForSearch(property.PropertyType, exceptionList, logger);

          if (propertiesForSearch == null)
            return exceptionList;

          // При обработке сущности сначала выполняется поиск сущности для тех сущностей
          // создание дублей которых избыточно и не требуется. Если сущность найдена - возвращается сущность, иначе создается новая.
          variableForParameters = MethodCall(property.PropertyType, Constants.EntityActions.FindEntity, propertiesForSearch, this, false, exceptionList, logger);

          if (variableForParameters == null)
            variableForParameters = MethodCall(property.PropertyType, Constants.EntityActions.CreateEntity, propertiesForSearch, this, exceptionList, isBatch, logger);
        }
        else
        {
          if (!NamingParameters.ContainsKey(options.ExcelName))
            continue;

          variableForParameters = NamingParameters[options.ExcelName].Trim();
          if (options.IsRequired())
          {
            if (CheckPropertyNull(options, variableForParameters, Constants.Resources.EmptyColumn, exceptionList, logger) == Constants.ErrorTypes.Error)
              return exceptionList;
          }

          // Свойства с типом Дата везде обрабатываются одинаково, поэтому можно преобразовать в общем коде.
          if (property.PropertyType == typeof(DateTimeOffset?))
          {
            variableForParameters = TransformDateTime((string)variableForParameters, options, exceptionList, logger);
            if (variableForParameters == null && options.IsRequired())
              return exceptionList;
          }

          // Работа с полями-сущностями.
          if (options.Type == PropertyType.Entity || options.Type == PropertyType.EntityWithCreate)
          {
            var entityName = (string)variableForParameters;
            // баг - при поиске необязательного ссылочного объекта, у которого название для поиска является обязателньым - ошибка становится критичной
            // hack: если необязательное поле пустое - пропускаем, нет смысла дальше искать
            if (!options.IsRequired() && string.IsNullOrEmpty(entityName))
            {
              ResultValues.Add(property.Name, null);
              continue;
            }
            
            // Добавляем поля и значения для поиска или создания сущностей.
            var propertiesForSearch = GetPropertiesForSearch(property.PropertyType, exceptionList, logger);            

            if (propertiesForSearch == null)
              propertiesForSearch = new Dictionary<string, string>();

            // Добавляем поле под служебным наименованием и его значение для
            // работы со связанными с импортируемой сущностью другими сущностями в системе
            // (поиск и создание, к примеру, головная организация, регионы, пользователи), чтобы явно можно было определить
            // их наименование и не спутать с наименованием (полем NAME) обрабатываемой в импорте сущности.
            propertiesForSearch.TryAdd(Constants.KeyAttributes.CustomFieldName, entityName);
            // Пробуем найти сущность в системе.
            variableForParameters = MethodCall(property.PropertyType, Constants.EntityActions.FindEntity, propertiesForSearch, this, false, exceptionList, logger);

            // Создаем сущность, если не удалось найти.
            if (options.Type == PropertyType.EntityWithCreate && variableForParameters == null && !string.IsNullOrEmpty(entityName))
              variableForParameters = MethodCall(property.PropertyType, Constants.EntityActions.CreateEntity, propertiesForSearch, this, exceptionList, isBatch, logger);

            if (CheckPropertyNull(options, variableForParameters, Constants.Resources.EmptyProperty, exceptionList, logger) == Constants.ErrorTypes.Error)
              return exceptionList;
          }
        }

        ResultValues.Add(property.Name, variableForParameters);
      }

      // Специфичные преобразования / проверки полей, которые нет возможности унифицировать.
      // Если метод вернул true, значит при проверках была добавлена ошибка, сущность не может быть загружена.
      var hasTransformationErrors = FillProperies(exceptionList, logger);
      if (hasTransformationErrors)
        return exceptionList;

      // Обновление сущности.
      try
      {
        var propertiesForCreate = GetPropertiesForSearch(EntityType, exceptionList, logger);
        if (ignoreDuplicates.ToLower() != Constants.ignoreDuplicates.ToLower())
          entity = (IEntityBase)MethodCall(EntityType, Constants.EntityActions.FindEntity, propertiesForCreate, this, true, exceptionList, logger);
        if (entity == null)
        {
          isNewEntity = true;
          entity = (IEntityBase)Activator.CreateInstance(EntityType);
        }

        // Заполнение полей.
        UpdateProperties(entity);

        // Создание сущности.
        entity = (IEntityBase)MethodCall(EntityType, Constants.EntityActions.CreateOrUpdate, entity, isNewEntity, isBatch, exceptionList, logger);

        // При необходимость дозаполнить свойства-коллекции.
        if(entity != null)
          FillCollections(exceptionList, logger);
      }
      catch (Exception ex)
      {
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = ex.Message });

        return exceptionList;
      }

      return exceptionList;
    }

    /// <summary>
    /// Получить значения полей шаблона для создания свойства-сущности.
    /// </summary>
    /// <param name="entityProperty">Свойство-сущность.</param>
    /// <param name="exceptionList">Список исключений.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Список пар {Название свойства - Значение} из свойств, по которым должна искаться / создаваться сущность. </returns>
    /// Метод собирает все свойства по иерархии, помеченные ForSearch  и их значения, 
    /// логика их использования и "отбрасывания" ненужных лежит в частном методе сущности.
    /// У сущности может не быть свойств, соответствующих полю шаблона (например, ФИО для Персоны контакта, нет у Контакта), их значения нужно куда-то сохранить.
    protected Dictionary<string, string> GetPropertiesForSearch(Type type, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var properties = type.GetProperties();
      var propertiesForSearch = new Dictionary<string, string>();
      var variableForParameters = string.Empty;
      foreach (var property in properties)
      {
        // Для свойства-сущности поиск может вестись по Наименованию, но в шаблоне оно будет называться по-другому,
        // поэтому обрабатывается отдельно.
        if (property.Name == Constants.KeyAttributes.Name && ResultValues.ContainsKey(Constants.KeyAttributes.Name))
          variableForParameters = (string)ResultValues[Constants.KeyAttributes.Name];
        else
        {
          var options = BusinessLogic.GetPropertyOptions(property);
          if (options == null || options.Characters != AdditionalCharacters.ForSearch || !NamingParameters.ContainsKey(options.ExcelName))
            continue;

          variableForParameters = NamingParameters[options.ExcelName].Trim();
          if (options.IsRequired())
          {
            if (CheckPropertyNull(options, variableForParameters, Constants.Resources.EmptyColumn, exceptionList, logger) == Constants.ErrorTypes.Error)
              return null;
          }
        }

        propertiesForSearch.Add(property.Name, variableForParameters);
      }

      return propertiesForSearch;
    }

    /// <summary>
    /// Проверить свойство на пустоту и обработать ошибки.
    /// </summary>
    /// <param name="options">Атрибуты свойства.</param>
    /// <param name="value">Значение свойства.</param>
    /// <param name="message">Текст сообщения при ошибке.</param>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Тип ошибки: ошибка, предупреждение или Debug, если ошибок нет.</returns>
    private string CheckPropertyNull(PropertyOptions options, object value, string message, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      string errorType = Constants.ErrorTypes.Debug;
      if (value == null || (value is string && string.IsNullOrEmpty((string)value)))
      {
        if (options.IsRequired())
          errorType = BusinessLogic.GetErrorResult(exceptionList, logger, message, options.ExcelName);
        else
          errorType = BusinessLogic.GetWarnResult(exceptionList, logger, message, options.ExcelName);
      }

      return errorType;
    }

    /// <summary>
    /// Заполнение /обновление полей сущности.
    /// </summary>
    /// <param name="entity">Сущность RX для заполнения.</param>
    private void UpdateProperties(IEntityBase entity)
    {
      var entityProperties = EntityType.GetProperties();
      foreach (var property in entityProperties)
      {
        if (ResultValues.ContainsKey(property.Name))
        {
          var options = BusinessLogic.GetPropertyOptions(property);
          if (options?.Characters == AdditionalCharacters.Collection)
            continue;

          if (property.PropertyType == typeof(double))
            property.SetValue(entity, Convert.ToDouble(ResultValues[property.Name], CultureInfo.InvariantCulture));
          else
            property.SetValue(entity, ResultValues[property.Name]);
        }
      }
    }

    /// <summary>
    /// Преобразовать зачение в дату.
    /// </summary>
    /// <param name="value">Значение.</param>
    /// <param name="style">Стиль преобразования числовой строки.</param>
    /// <param name="culture">Культура.</param>
    /// <returns>Преобразованная дата.</returns>
    /// <exception cref="FormatException" />
    private DateTimeOffset ParseDate(string value, NumberStyles style, CultureInfo culture)
    {
      if (!string.IsNullOrEmpty(value))
      {
        DateTimeOffset date;
        if (DateTimeOffset.TryParse(value.Trim(), culture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out date))
          return date;

        var dateDouble = 0.0;
        if (double.TryParse(value.Trim(), style, culture, out dateDouble))
          return new DateTimeOffset(DateTime.FromOADate(dateDouble), TimeSpan.Zero);

        throw new FormatException("Неверный формат строки.");
      }
      else
        return DateTimeOffset.MinValue;
    }

    /// <summary>
    /// Преобразование даты с учетом атрибутов свойства.
    /// </summary>
    /// <param name="value">Строка даты.</param>
    /// <param name="options">Атрибуты свойства.</param>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Преобразованная дата.</returns>
    protected DateTimeOffset? TransformDateTime(string value, PropertyOptions options, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
      var culture = CultureInfo.CreateSpecificCulture("en-GB");

      var message = @"Не удалось обработать значение в поле ""{0}"" ""{1}"".";
      try
      {
        return ParseDate(value, style, culture);
      }
      catch
      {
        if (options.IsRequired())
          BusinessLogic.GetErrorResult(exceptionList, logger, message, options.ExcelName, value);
        else
          BusinessLogic.GetWarnResult(exceptionList, logger, message, options.ExcelName, value);

        return null;
      }
    }

    /// <summary>
    /// Вызов метода для заданного типа сущности.
    /// </summary>
    /// <param name="type">Тип сущности.</param>
    /// <param name="methodName">Имя вызываемого метода.</param>
    /// <param name="paramsForMethod">Параметры вызываемого метода.</param>
    /// <returns>Результат выполнения метода.</returns>
    protected object MethodCall(Type type, string methodName, params object[] paramsForMethod)
    {
      MethodInfo method = type.GetMethod(methodName);
      return method.Invoke(null, paramsForMethod);
    }

    /// <summary>
    /// Получить имя сущности для заполнения (оно может составляться из нескольких столбцов шаблона).
    /// </summary>
    /// <param name="entity">Сущность со всеми параметрами загрузки. (Предполагается, что при заполнении имени все поля уже считаны.)</param>
    /// <returns>Наименование.</returns>
    protected virtual string GetName()
    {
      return string.Empty;
    }

    /// <summary>
    /// Специфичное заполнение / преобразование / проверка полей сущность, которую нельзя унифицировать.
    /// </summary>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>True, если были ошибки заполнения свойств, иначе false.</returns>
    protected virtual bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      return false;
    }

    /// <summary>
    /// Заполнение свойств-коллекций.
    /// </summary>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="logger">Логировщик.</param>
    protected virtual void FillCollections(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      // По необходимости переопределяется в дочерних классах.
    }

    /// <summary>
    /// Проверка валидности реквизитов организации.
    /// </summary>
    /// <param name="nonresident">Признак компании-резидента.</param>
    /// <param name="tin">ИНН.</param>
    /// <param name="trrc">КПП.</param>
    /// <param name="psrn">ОГРН.</param>
    /// <param name="nceo">ОКПО.</param>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Результат проверки реквизитов на валидность.</returns>
    protected virtual bool CheckCompanyRequsite(bool nonresident, string tin, string trrc, string psrn, string nceo, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      bool isExistNotValidProps = false;

      // Проверка ИНН.
      var resultTIN = BusinessLogic.CheckTin(tin, true, nonresident);

      if (!string.IsNullOrEmpty(resultTIN))
      {
        var message = "Компания не может быть импортирована. Некорректный ИНН. Наименование: \"{0}\", ИНН: {1}. {2}";
        BusinessLogic.GetErrorResult(exceptionList, logger, message, (string)ResultValues[Constants.KeyAttributes.Name], tin, resultTIN);
        isExistNotValidProps = true;
      }

      // Проверка КПП.
      var resultTRRC = BusinessLogic.CheckTrrcLength(trrc, nonresident);

      if (!string.IsNullOrEmpty(resultTRRC))
      {
        var message = "Компания не может быть импортирована. Некорректный КПП. Наименование: \"{0}\", КПП: {1}. {2}";
        BusinessLogic.GetErrorResult(exceptionList, logger, message, (string)ResultValues[Constants.KeyAttributes.Name], trrc, resultTRRC);
        isExistNotValidProps = true;
      }

      // Проверка ОГРН.
      var resultPSRN = BusinessLogic.CheckPsrnLength(psrn, nonresident);

      if (!string.IsNullOrEmpty(resultPSRN))
      {
        var message = "Компания не может быть импортирована. Некорректный ОГРН. Наименование: \"{0}\", ОГРН: {1}. {2}";
        BusinessLogic.GetErrorResult(exceptionList, logger, message, (string)ResultValues[Constants.KeyAttributes.Name], psrn, resultPSRN);
        isExistNotValidProps = true;
      }

      // Проверка ОКПО.
      var resultNCEO = BusinessLogic.CheckNceoLength(nceo, nonresident);

      if (!string.IsNullOrEmpty(resultNCEO))
      {
        var message = "Компания не может быть импортирована. Некорректный ОКПО. Наименование: \"{0}\", ОКПО: {1}. {2}";
        BusinessLogic.GetErrorResult(exceptionList, logger, message, (string)ResultValues[Constants.KeyAttributes.Name], nceo, resultNCEO);
        isExistNotValidProps = true;
      }

      return isExistNotValidProps;
    }
  }
}
