using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Входящее письмо")]
  public class IIncomingLetters : IOfficialDocuments
  {
    private DateTimeOffset? dated;
    private DateTimeOffset? registrationDate;

    [PropertyOptions("№", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public string RegistrationNumber { get; set; }

    [PropertyOptions("Дата регистрации", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public DateTimeOffset? RegistrationDate
    {
      get { return registrationDate; }
      set { registrationDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("Письмо от", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public DateTimeOffset? Dated
    {
      get { return dated; }
      set { dated = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("Номер входящий", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string InNumber { get; set; }

    public bool IsManyAddressees { get; set; }
    public string ManyAddresseesPlaceholder { get; set; }
    public string ManyAddresseesLabel { get; set; }

    [PropertyOptions("Адресат", RequiredType.NotRequired, PropertyType.Entity)]
    public IEmployees Addressee { get; set; }

    public IEmployees Assignee { get; set; }
    public IBusinessUnits BusinessUnit { get; set; }

    [PropertyOptions("Корреспондент", RequiredType.Required, PropertyType.EntityWithCreate, AdditionalCharacters.ForSearch)]
    public ICounterparties Correspondent { get; set; }

    public IEmployees ResponsibleForReturnEmployee { get; set; }

    [PropertyOptions("Способ доставки", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IMailDeliveryMethods DeliveryMethod { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      IIncomingLetters incomingLetters = null;
      var docRegisterId = propertiesForSearch[Constants.KeyAttributes.DocumentRegister];
      var regNumber = propertiesForSearch[Constants.KeyAttributes.RegistrationNumber];

      if (GetDate(propertiesForSearch[Constants.KeyAttributes.RegistrationDate], out var registrationDate) &&
          int.TryParse(docRegisterId, out int documentRegisterId))
      {
        incomingLetters = BusinessLogic.GetEntityWithFilter<IIncomingLetters>(x => x.RegistrationNumber == regNumber &&
          x.RegistrationDate == registrationDate &&
          x.DocumentRegister.Id == documentRegisterId,
        exceptionList, logger);
      }

      return incomingLetters;
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
      {
        entity = BusinessLogic.CreateEntity((IIncomingLetters)entity, exceptionList, logger, isBatch);
        return ((IIncomingLetters)entity)?.UpdateLifeCycleState(Constants.AttributeValue[Constants.KeyAttributes.Status], isBatch);
      }
      else
        return BusinessLogic.UpdateEntity((IIncomingLetters)entity, exceptionList, logger);
    }
  }
}
