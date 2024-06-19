﻿using ImportData.IntegrationServicesClient;
using ImportData.IntegrationServicesClient.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ImportData
{
  public class Entity
  {
    public string[] Parameters;
    public Dictionary<string, string> ExtraParameters;
    public int PropertiesCount = 0;

    public Dictionary<string, string> NamingParameters { get; set; }
    public Dictionary<string, object> ResultValues { get; set; }
    protected virtual Type EntityType { get; }

    /// <summary>
    /// Получить наименование число запрашиваемых параметров.
    /// </summary>
    /// <returns>Число запрашиваемых параметров.</returns>
    public virtual int GetPropertiesCount()
    {
      return PropertiesCount;
    }

    /// <summary>
    /// Сохранение сущности в RX.
    /// </summary>
    /// <param name="logger">Логировщик.</param>
    /// <param name="shift">Сдвиг по горизонтали в XLSX документе. Необходим для обработки документов, составленных из элементов разных сущностей.</param>
    /// <returns>Список ошибок.</returns>
    public virtual IEnumerable<Structures.ExceptionsStruct> SaveToRX(NLog.Logger logger, bool supplementEntity, string ignoreDuplicates, int shift = 0)
    {
      return new List<Structures.ExceptionsStruct>();
    }

    public virtual IEnumerable<Structures.ExceptionsStruct> Save(NLog.Logger logger, bool supplementEntity, string ignoreDuplicates)
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
          // Важно: скорее всего тут не только создание, а еще поиск
          variableForParameters = MethodCall(property.PropertyType, "CreateEntity", propertiesForSearch, this, exceptionList, logger);
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
            // ВАЖНО: есть сущности, которые ищутся не по имени, для них подумать, как доработать GetPropertiesForSearch,
            // он должен в зависимости от сущности брать все поля по иерархии или только поля сущности
            // пока так
            var propertiesForSearch = new Dictionary<string, string>();
            var entityName = (string)variableForParameters;
            propertiesForSearch.Add("Name", entityName);
            variableForParameters = MethodCall(property.PropertyType, "FindEntity", propertiesForSearch, this, false, exceptionList, logger);
            if (options.Type == PropertyType.EntityWithCreate && variableForParameters == null && !string.IsNullOrEmpty(entityName))
              variableForParameters = MethodCall(property.PropertyType, "CreateEntity", propertiesForSearch, this, exceptionList, logger);

            if (CheckPropertyNull(options, variableForParameters, Constants.Resources.EmptyProperty, exceptionList, logger) == Constants.ErrorTypes.Error)
              return exceptionList;
          }
        }

        ResultValues.Add(property.Name, variableForParameters);
      }

      // Специфичные преобразования / проверки полей, которые нет возможности унифицировать.
      // Если метод вернул true, значит при проверках была добавлена ошибка, сущность не может быть загружена.
      var hasTransformationErrors = (bool)MethodCall(EntityType, "FillProperies", this, exceptionList, logger);
      if (hasTransformationErrors)
        return exceptionList;

      // Обновление сущности.
      try
      {
        IEntityBase entity = null;
        var propertiesForCreate = GetPropertiesForSearch(EntityType, exceptionList, logger);
        var isNewEntity = false;
        if (ignoreDuplicates.ToLower() != Constants.ignoreDuplicates.ToLower())
          entity = (IEntityBase)MethodCall(EntityType, "FindEntity", propertiesForCreate, this, true, exceptionList, logger);
        if (entity == null)
        {
          isNewEntity = true;
          entity = (IEntityBase)Activator.CreateInstance(EntityType);
        }

        // Заполнение полей.
        UpdateProperties(entity);

        // Создание сущности.
        MethodCall(EntityType, "CreateOrUpdate", entity, isNewEntity, exceptionList, logger);
      }
      catch (Exception ex)
      {
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = ex.Message });

        return exceptionList;
      }

      return new List<Structures.ExceptionsStruct>();
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
    Dictionary<string, string> GetPropertiesForSearch(Type type, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
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
    string CheckPropertyNull(PropertyOptions options, object value, string message, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      string errorType = Constants.ErrorTypes.Debug;
      if (value == null || (value is string && string.IsNullOrEmpty((string)value)))
      {
        if (options.IsRequired())
          errorType = GetErrorResult(exceptionList, logger, message, options.ExcelName);
        else
          errorType = GetWarnResult(exceptionList, logger, message, options.ExcelName);
      }

      return errorType;
    }

    /// <summary>
    /// Добавить ошибку.
    /// </summary>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="logger">Логировщик.</param>
    /// <param name="message">Текст сообщения при ошибке.</param>
    /// <param name="propertyName">Значения для подстановки в текст сообщения об ошибке.</param>
    /// <returns>Тип ошибки Error/</returns>
    public string GetErrorResult(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger, string message, params string[] propertyName)
    {
      message = string.Format(message, propertyName);
      exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
      logger.Error(message);
      return Constants.ErrorTypes.Error;
    }

    /// <summary>
    /// Добавить предупреждение.
    /// </summary>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="logger">Логировщик.</param>
    /// <param name="message">Текст предупреждения.</param>
    /// <param name="propertyName">Значения для подстановки в текст предупреждения.</param>
    /// <returns>Тип ошибки Warn/</returns>
    public string GetWarnResult(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger, string message, params string[] propertyName)
    {
      message = string.Format(message, propertyName);
      exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
      logger.Error(message);
      return Constants.ErrorTypes.Warn;
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
          property.SetValue(entity, ResultValues[property.Name]);
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
    public DateTimeOffset ParseDate(string value, NumberStyles style, CultureInfo culture)
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
    public DateTimeOffset? TransformDateTime(string value, PropertyOptions options, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
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
          GetErrorResult(exceptionList, logger, message, options.ExcelName, value);
        else
          GetWarnResult(exceptionList, logger, message, options.ExcelName, value);

        return null;
      }
    }

    /// <summary>
    /// Получить начало дня.
    /// </summary>
    /// <param name="dateTimeOffset">Дата-время.</param>
    /// <returns>Начало дня.</returns>
    public DateTimeOffset BeginningOfDay(DateTimeOffset dateTimeOffset)
    {
      return new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, 0, 0, 0, dateTimeOffset.Offset);
    }

    /// <summary>
    /// Вызов метода для заданного типа сущности.
    /// </summary>
    /// <param name="type">Тип сущности.</param>
    /// <param name="methodName">Имя вызываемого метода.</param>
    /// <param name="paramsForMethod">Параметры вызываемого метода.</param>
    /// <returns>Результат выполнения метода.</returns>
    public object MethodCall(Type type, string methodName, params object[] paramsForMethod)
    {
      MethodInfo method = type.GetMethod(methodName);
      return method.Invoke(null, paramsForMethod);
    }
  }
}
