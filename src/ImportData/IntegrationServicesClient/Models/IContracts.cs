using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Договор")]
  public class IContracts : IContractualDocuments
  {
    private DateTimeOffset? registrationDate;

    [PropertyOptions("№ договора", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public string RegistrationNumber { get; set; }

    [PropertyOptions("Дата договора", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public DateTimeOffset? RegistrationDate
    {
      get { return registrationDate; }
      set { registrationDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("ИД журнала регистрации", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    new public IDocumentRegisters DocumentRegister { get; set; }

    new public static IContracts FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      IContracts contracts = null;
      var regNumber = propertiesForSearch[Constants.KeyAttributes.RegistrationNumber];
      var counterpartyName = propertiesForSearch[Constants.KeyAttributes.Counterparty];
      var counterparty = BusinessLogic.GetEntityWithFilter<ICounterparties>(x => x.Name == counterpartyName, exceptionList, logger);
      var docRegisterId = propertiesForSearch[Constants.KeyAttributes.DocumentRegister];

      if (GetDate(propertiesForSearch[Constants.KeyAttributes.RegistrationDate], out var registrationDate) &&
        int.TryParse(docRegisterId, out int documentRegisterId))
      {
        var documentRegister = BusinessLogic.GetEntityWithFilter<IDocumentRegisters>(x => x.Id == documentRegisterId, exceptionList, logger);
        contracts = BusinessLogic.GetEntityWithFilter<IContracts>(x => x.RegistrationNumber != null &&
          x.RegistrationNumber == regNumber &&
          x.RegistrationDate == registrationDate &&
          x.Counterparty.Id == counterparty.Id &&
          x.DocumentRegister.Id == documentRegister.Id, exceptionList, logger);
      }

      return contracts;
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
      {
        var lifeCycleState = ((IContracts)entity).LifeCycleState;
        entity = BusinessLogic.CreateEntity((IContracts)entity, exceptionList, logger, isBatch);
        return ((IContracts)entity)?.UpdateLifeCycleState(lifeCycleState, isBatch);
      }
      else
      {
        return BusinessLogic.UpdateEntity((IContracts)entity, exceptionList, logger);
      }
    }
  }

}
