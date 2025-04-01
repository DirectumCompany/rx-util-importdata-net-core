using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{  
  [EntityName("Акт выполненных работ")]
  public class IContractStatements : IDocflowAccountingDocumentBases
  {
    new public static IContractStatements FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      IContractStatements document = null;
      var regNumber = propertiesForSearch[Constants.KeyAttributes.RegistrationNumber];
      var counterpartyName = propertiesForSearch[Constants.KeyAttributes.Counterparty];
      var counterparty = BusinessLogic.GetEntityWithFilter<ICounterparties>(x => x.Name == counterpartyName, exceptionList, logger);
      var docRegisterId = propertiesForSearch[Constants.KeyAttributes.DocumentRegister];

      if (GetDate(propertiesForSearch[Constants.KeyAttributes.RegistrationDate], out var registrationDate) &&
        int.TryParse(docRegisterId, out int documentRegisterId))
      {
        var documentRegister = BusinessLogic.GetEntityWithFilter<IDocumentRegisters>(x => x.Id == documentRegisterId, exceptionList, logger);
        document = BusinessLogic.GetEntityWithFilter<IContractStatements>(x => x.RegistrationNumber != null &&
          x.RegistrationNumber == regNumber &&
          x.RegistrationDate == registrationDate &&
          x.Counterparty.Id == counterparty.Id &&
          x.DocumentRegister.Id == documentRegister.Id, exceptionList, logger);
      }

      return document;
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
      {
        var lifeCycleState = ((IContractStatements)entity).LifeCycleState;
        entity = BusinessLogic.CreateEntity((IContractStatements)entity, exceptionList, logger, isBatch);
        return ((IContractStatements)entity)?.UpdateLifeCycleState(lifeCycleState, isBatch);
      }
      else
      {
        return BusinessLogic.UpdateEntity((IContractStatements)entity, exceptionList, logger);
      }
    }
  }
}
