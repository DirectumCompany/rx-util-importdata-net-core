using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Способы доставки")]
  public class IMailDeliveryMethods : IEntity
  {
    public string Note { get; set; }
    public string Sid { get; set; }
    public string Status { get; set; }

    new public static IMailDeliveryMethods FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.CustomFieldName];

      return BusinessLogic.GetEntityWithFilter<IMailDeliveryMethods>(x => x.Name == name, exceptionList, logger);
    }
  }
}
