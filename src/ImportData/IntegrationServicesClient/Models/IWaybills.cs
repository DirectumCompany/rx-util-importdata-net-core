using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{  
  [EntityName("Накладная")]
  public class IWaybills : IDocflowAccountingDocumentBases
  {
    new public static IWaybills FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      IWaybills document = null;
      var regNumber = propertiesForSearch[Constants.KeyAttributes.RegistrationNumber];
      var counterpartyName = propertiesForSearch[Constants.KeyAttributes.Counterparty];
      var counterparty = BusinessLogic.GetEntityWithFilter<ICounterparties>(x => x.Name == counterpartyName, exceptionList, logger);
      var docRegisterId = propertiesForSearch[Constants.KeyAttributes.DocumentRegister];

      if (GetDate(propertiesForSearch[Constants.KeyAttributes.RegistrationDate], out var registrationDate) &&
        int.TryParse(docRegisterId, out int documentRegisterId))
      {
        var documentRegister = BusinessLogic.GetEntityWithFilter<IDocumentRegisters>(x => x.Id == documentRegisterId, exceptionList, logger);
        document = BusinessLogic.GetEntityWithFilter<IWaybills>(x => x.RegistrationNumber != null &&
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
        var lifeCycleState = ((IWaybills)entity).LifeCycleState;
        entity = BusinessLogic.CreateEntity((IWaybills)entity, exceptionList, logger, isBatch);
        return ((IWaybills)entity)?.UpdateLifeCycleState(lifeCycleState, isBatch);
      }
      else
      {
        return BusinessLogic.UpdateEntity((IWaybills)entity, exceptionList, logger);
      }
    }
  }
}
