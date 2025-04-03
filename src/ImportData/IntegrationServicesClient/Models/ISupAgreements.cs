using System;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Дополнительные соглашения")]
  public class ISupAgreements : IContractualDocuments
  {
    private DateTimeOffset? registrationDate;
    private DateTimeOffset? documentDate;
    private DateTimeOffset? validFrom;
    private DateTimeOffset? validTill;

    [PropertyOptions("№ доп. соглашения", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public string RegistrationNumber { get; set; }

    [PropertyOptions("Дата доп. соглашения", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public DateTimeOffset? RegistrationDate
    {
      get { return registrationDate; }
      set { registrationDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("№ договора", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    new public IOfficialDocuments LeadingDocument { get; set; }

    [PropertyOptions("Дата договора", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.Default)]
    new public DateTimeOffset? DocumentDate
    {
      get { return documentDate; }
      set { documentDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("Действует с", RequiredType.Required, PropertyType.Simple)]
    new public DateTimeOffset? ValidFrom
    {
      get { return validFrom; }
      set { validFrom = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("Действует по", RequiredType.Required, PropertyType.Simple)]
    new public DateTimeOffset? ValidTill
    {
      get { return validTill; }
      set { validTill = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("Сумма", RequiredType.Required, PropertyType.Simple)]
    new public double TotalAmount { get; set; }

    [PropertyOptions("Состояние", RequiredType.NotRequired, PropertyType.Simple)]
    new public string LifeCycleState { get; set; }

    [PropertyOptions("Валюта", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    new public ICurrencies Currency { get; set; }

    [PropertyOptions("Журнал регистрации", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    new public IDocumentRegisters DocumentRegister { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ISupAgreements supAgreement = null;
      var regNumber = propertiesForSearch[Constants.KeyAttributes.RegistrationNumber];
      var counterpartyName = propertiesForSearch[Constants.KeyAttributes.Counterparty];
      var counterparty = BusinessLogic.GetEntityWithFilter<ICounterparties>(x => x.Name == counterpartyName, exceptionList, logger);
      var docRegisterId = propertiesForSearch[Constants.KeyAttributes.DocumentRegister];

      if (GetDate(propertiesForSearch[Constants.KeyAttributes.RegistrationDate], out var registrationDate) &&
        int.TryParse(docRegisterId, out int documentRegisterId))
      {
        supAgreement = BusinessLogic.GetEntityWithFilter<ISupAgreements>(x => x.RegistrationNumber != null &&
          x.RegistrationNumber == regNumber && x.Counterparty.Id == counterparty.Id &&
          x.RegistrationDate == registrationDate &&
          x.DocumentRegister.Id == documentRegisterId, 
          exceptionList, logger);
      }
      return supAgreement;
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
      {
        var lifeCycleState = ((ISupAgreements)entity).LifeCycleState;
        entity = BusinessLogic.CreateEntity((ISupAgreements)entity, exceptionList, logger, isBatch);
        return ((ISupAgreements)entity)?.UpdateLifeCycleState(lifeCycleState, isBatch);
      }
      else
      {        
        return BusinessLogic.UpdateEntity((ISupAgreements)entity, exceptionList, logger);
      }
    }
  }
}
