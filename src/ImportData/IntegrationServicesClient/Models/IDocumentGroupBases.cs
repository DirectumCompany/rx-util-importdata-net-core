using System;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Группа документов")]
  public class IDocumentGroupBases : IEntity
  {
    [PropertyOptions("Примечание", RequiredType.NotRequired, PropertyType.Simple)]
    public string Note { get; set; }
    public string Status { get; set; }

    [PropertyOptions("Виды документов", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.Collection)]
    public IEnumerable<IDocumentKinds> DocumentKinds { get; set; }

    new public static IDocumentGroupBases CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.DocumentGroup];

      return BusinessLogic.CreateEntity<IDocumentGroupBases>(new IDocumentGroupBases()
      {
        Name = name,
        Status = Constants.AttributeValue[Constants.KeyAttributes.Status]
      }, exceptionList, logger);
    }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.CustomFieldName];

      return BusinessLogic.GetEntityWithFilter<IDocumentGroupBases>(x => x.Name == name, exceptionList, logger);
    }

  }
}
