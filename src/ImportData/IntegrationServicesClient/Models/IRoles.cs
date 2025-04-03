using ImportData.Entities.Databooks;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Роль")]
  public class IRoles : IEntity
  {
    [PropertyOptions("Описание", RequiredType.NotRequired, PropertyType.Simple)]
    public string Description { get; set; }

    // По умолчанию false, если нужно установить флаг, то нужно сделать это вручную в RX.
    public bool IsSingleUser { get; set; } = false;

    public string Status { get; set; }

    [PropertyOptions("Состав участников", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.Collection)]
    public IEnumerable<IRecipients> RecipientLinks { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];
      return BusinessLogic.GetEntityWithFilter<IRoles>(x => x.Name == name, exceptionList, logger);
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        return BusinessLogic.CreateEntity((IRoles)entity, exceptionList, logger);
      else
      {
        BusinessLogic.GetErrorResult(exceptionList, logger, Constants.Resources.EntityNotLoaded, entity.Name);
        return null;
      }
    }

    public void AddRecipient(IRecipients recipient, NLog.Logger logger)
    {
      try
      {
        var result = Client.Instance().For<IRoles>()
         .Key(this)
         .NavigateTo(nameof(RecipientLinks))
         .Set(new { Member = recipient })
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
