namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Внутренний документ")]
  public class IInternalDocumentBases : IOfficialDocuments
  {
    [PropertyOptions("Исполнитель", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IEmployees Assignee { get; set; }

    [PropertyOptions("Наша организация", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IBusinessUnits BusinessUnit { get; set; }
  }
}
