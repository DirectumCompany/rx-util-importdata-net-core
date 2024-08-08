using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Виды документов")]
  public class IDocumentKinds : IEntity
  {
    public string Note { get; set; }
    public int DeadlineInDays { get; set; }
    public string ShortName { get; set; }
    public int DeadlineInHours { get; set; }
    public bool GenerateDocumentName { get; set; }
    public bool AutoNumbering { get; set; }
    public bool ProjectsAccounting { get; set; }
    public bool GrantRightsToProject { get; set; }
    public bool IsDefault { get; set; }
    public string Code { get; set; }
    public string DocumentFlow { get; set; }
    public string NumberingType { get; set; }
    public string Status { get; set; }
    public IDocumentType DocumentType { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var kind = propertiesForSearch[Constants.KeyAttributes.CustomFieldName];

      return BusinessLogic.GetEntityWithFilter<IDocumentKinds>(x => x.Name == kind, exceptionList, logger);
    }
  }
}
