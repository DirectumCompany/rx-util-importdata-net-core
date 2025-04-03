using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Группа регистрации")]
  public class IRegistrationGroups : IRecipients
  {
    public string Index { get; set; }
    public bool CanRegisterIncoming { get; set; }
    public bool CanRegisterOutgoing { get; set; }
    public bool CanRegisterInternal { get; set; }
    public bool CanRegisterContractual { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
       propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.Name];

      return BusinessLogic.GetEntityWithFilter<IRegistrationGroups>(x => x.Name == name, exceptionList, logger);
    }
  }
}
