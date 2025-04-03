using System;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Категория договора")]
  public class IContractCategories : IDocumentGroupBases
  {
    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
        propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.Name];

      return BusinessLogic.GetEntityWithFilter<IContractCategories>(x => x.Name == name, exceptionList, logger);
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        return BusinessLogic.CreateEntity((IContractCategories)entity, exceptionList, logger);
      else
      {
        BusinessLogic.GetErrorResult(exceptionList, logger, Constants.Resources.EntityNotLoaded, entity.Name);
        return null;
      }
    }

    public void AddDocumentKind(IDocumentKinds kind, NLog.Logger logger)
    {
      try
      {
        var result = Client.Instance().For<IContractCategories>()
         .Key(this)
         .NavigateTo(nameof(DocumentKinds))
         .Set(new { DocumentKind =  kind })
         .InsertEntryAsync().Result;
      }
      catch (Exception ex)
      {
        logger.Error(ex);
        throw;
      }
    }

  }
}
