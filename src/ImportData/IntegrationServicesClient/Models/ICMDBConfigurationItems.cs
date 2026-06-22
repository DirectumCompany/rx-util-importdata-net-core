using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ImportData.IntegrationServicesClient.Models
{
  public class ICMDBConfigurationItems : IEntity
  {
    [EntityName("Конфигурационная единица")]

    [PropertyOptions("Код", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string Code { get; set; }

    [PropertyOptions("Группа КЕ", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public ICMDBConfigurationItemCategories ConfigurationItemCategory { get; set; }

   [PropertyOptions("Состояние КЕ", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
   public ICMDBConfigurationItemLifeCycleStates LifeCycleState { get; set; }

    [PropertyOptions("Тип КЕ", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public ICMDBConfigurationItemKinds ConfigurationItemKind { get; set; }

    [PropertyOptions("Закрепленный сотрудник", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IEmployees AssignedEmployee { get; set; }

    [PropertyOptions("Производитель", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string Manufacturer { get; set; }

    [PropertyOptions("Серийный номер", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string SerialNumber { get; set; }

    [PropertyOptions("Ответственный за КЕ", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IEmployees Responsible { get; set; }

    [PropertyOptions("Версия", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string Version { get; set; }

    [PropertyOptions("Примечание", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string Note { get; set; }

    [PropertyOptions("Адрес", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string LocationsAddress { get; set; }

    [PropertyOptions("Контактные данные", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string ContactInfo { get; set; }

    [PropertyOptions("Аппаратные требования", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string HardwareRequirements { get; set; }

    [PropertyOptions("Лицензия", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string License { get; set; }

    [PropertyOptions("Модель", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string Model { get; set; }

    [PropertyOptions("Потребляемая мощность(Вт)", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string PowerConsumption { get; set; }

    [PropertyOptions("IP-адрес", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string IPAddress { get; set; }

    [PropertyOptions("МАС-адрес", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string MACAddress { get; set; }

    [PropertyOptions("Номер порта", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string PortNumber { get; set; }

    [PropertyOptions("Скорость порта", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string PortSpeed { get; set; }

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
      var codeName = propertiesForSearch[Constants.KeyAttributes.Code];
      var categoryName = propertiesForSearch[Constants.KeyAttributes.ConfigurationItemCategory];
      var responsibleName = propertiesForSearch[Constants.KeyAttributes.AssignedEmployee];

      return BusinessLogic.GetEntityWithFilter<ICMDBConfigurationItems>(x => x.Name == name && x.Code == codeName && x.ConfigurationItemCategory.Name == categoryName && x.AssignedEmployee.Name == responsibleName, exceptionList, logger);
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
    new public static IEntity CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];

      var responsibleName = propertiesForSearch[Constants.KeyAttributes.Responsible];
      var responsible = BusinessLogic.GetEntityWithFilter<IEmployees>(x => x.Name == responsibleName, exceptionList, logger);

      var assignedEmployeeName = propertiesForSearch[Constants.KeyAttributes.AssignedEmployee];
      var assignedEmployee = BusinessLogic.GetEntityWithFilter<IEmployees>(x => x.Name == assignedEmployeeName, exceptionList, logger);

      var configurationItemCategoryName = propertiesForSearch[Constants.KeyAttributes.ConfigurationItemCategory];
      var configurationItemCategory = BusinessLogic.GetEntityWithFilter<ICMDBConfigurationItemCategories>(x => x.Name == configurationItemCategoryName, exceptionList, logger);

      var configurationItemKindName = propertiesForSearch[Constants.KeyAttributes.ConfigurationItemKind];
      var configurationItemKind = BusinessLogic.GetEntityWithFilter<ICMDBConfigurationItemKinds>(x => x.Name == configurationItemKindName, exceptionList, logger);

      var lifeCycleStateName = propertiesForSearch[Constants.KeyAttributes.LifeCycleState];
      var lifeCycleStateCi = BusinessLogic.GetEntityWithFilter<ICMDBConfigurationItemLifeCycleStates>(x => x.Name == lifeCycleStateName, exceptionList, logger);

      return BusinessLogic.CreateEntity<ICMDBConfigurationItems>(new ICMDBConfigurationItems()
      {
        Name = name,
        Code = propertiesForSearch[Constants.KeyAttributes.Code],
        ConfigurationItemCategory = configurationItemCategory,
        LifeCycleState = lifeCycleStateCi,
        ConfigurationItemKind = configurationItemKind,
        AssignedEmployee = assignedEmployee,
        Manufacturer = propertiesForSearch[Constants.KeyAttributes.Manufacturer],
        SerialNumber = propertiesForSearch[Constants.KeyAttributes.SerialNumber],
        Responsible = responsible,
        Version = propertiesForSearch[Constants.KeyAttributes.Version],
        Note = propertiesForSearch[Constants.KeyAttributes.Note],
        LocationsAddress = propertiesForSearch[Constants.KeyAttributes.LocationsAddress],
        ContactInfo = propertiesForSearch[Constants.KeyAttributes.ContactInfo],
        HardwareRequirements = propertiesForSearch[Constants.KeyAttributes.HardwareRequirements],
        License = propertiesForSearch[Constants.KeyAttributes.License],
        Model = propertiesForSearch[Constants.KeyAttributes.Model],
        PowerConsumption = propertiesForSearch[Constants.KeyAttributes.PowerConsumption],
        IPAddress = propertiesForSearch[Constants.KeyAttributes.IPAddress],
        MACAddress = propertiesForSearch[Constants.KeyAttributes.MACAddress],
        PortNumber = propertiesForSearch[Constants.KeyAttributes.PortNumber],
        PortSpeed = propertiesForSearch[Constants.KeyAttributes.PortSpeed],

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
    new public static IEntityBase CreateOrUpdate(ICMDBConfigurationItems entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        return BusinessLogic.CreateEntity((ICMDBConfigurationItems)entity, exceptionList, logger);
      else
      {
        return BusinessLogic.UpdateEntity((ICMDBConfigurationItems)entity, exceptionList, logger);
      }
    }
  }
}
