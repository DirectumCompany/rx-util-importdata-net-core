using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Типы документов")]
  public class IDocumentType : IEntity
  {
    public string DocumentTypeGuid { get; set; }
    public string DocumentFlow { get; set; }
    public string Status { get; set; }
    public bool IsRegistrationAllowed { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.CustomFieldName];
      return BusinessLogic.GetEntityWithFilter<IDocumentType>(x => x.Name == name, exceptionList, logger);
    }

  }
}
