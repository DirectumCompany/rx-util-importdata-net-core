using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Официальный документ")]
  public class IOfficialDocuments : IElectronicDocuments
  {
    private DateTimeOffset? registrationDate;
    private DateTimeOffset? documentDate;
    private DateTimeOffset? placedToCaseFileDate;

    [PropertyOptions("Рег. №", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string RegistrationNumber { get; set; }

    [PropertyOptions("Рег. Дата", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public DateTimeOffset? RegistrationDate
    {
      get { return registrationDate; }
      set { registrationDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("Содержание", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string Subject { get; set; }

    [PropertyOptions("Примечание", RequiredType.NotRequired, PropertyType.Simple)]
    public string Note { get; set; }

    [PropertyOptions("Дата договора", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public DateTimeOffset? DocumentDate
    {
      get { return documentDate; }
      set { documentDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("Состояние", RequiredType.NotRequired, PropertyType.Simple)]
    public string LifeCycleState { get; set; }

    [PropertyOptions("Регистрация", RequiredType.Required, PropertyType.Simple)]
    public string RegistrationState { get; set; }

    [PropertyOptions("Журнал регистрации", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IDocumentRegisters DocumentRegister { get; set; }

    [PropertyOptions("Вид документа", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IDocumentKinds DocumentKind { get; set; }

    public IOfficialDocuments LeadingDocument { get; set; }

    [PropertyOptions("Категория", RequiredType.NotRequired, PropertyType.EntityWithCreate, AdditionalCharacters.ForSearch)]
    public IDocumentGroupBases DocumentGroup { get; set; }

    [PropertyOptions("Подразделение", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IDepartments Department { get; set; }

    public IEmployees DeliveredTo { get; set; }

    [PropertyOptions("Подписал", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IEmployees OurSignatory { get; set; }

    [PropertyOptions("Подготовил", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IEmployees PreparedBy { get; set; }

    [PropertyOptions("Дело", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public ICaseFiles CaseFile { get; set; }

    [PropertyOptions("Дата помещения", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public DateTimeOffset? PlacedToCaseFileDate
    {
      get { return placedToCaseFileDate; }
      set { placedToCaseFileDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      IOfficialDocuments officialDocument = null;
      var regNumber = propertiesForSearch[Constants.KeyAttributes.CustomFieldName];

      if (propertiesForSearch.ContainsKey(Constants.KeyAttributes.DocumentDate) && GetDate(propertiesForSearch[Constants.KeyAttributes.DocumentDate], out var documentDate))
      {
        officialDocument = BusinessLogic.GetEntityWithFilter<IOfficialDocuments>(x => x.RegistrationNumber != null &&
          x.RegistrationNumber == regNumber &&
          x.DocumentDate == documentDate, exceptionList, logger);
      }
      else
      {
        officialDocument = BusinessLogic.GetEntityWithFilter<IOfficialDocuments>(x => x.RegistrationNumber != null &&
            x.RegistrationNumber == regNumber, exceptionList, logger);
      }

      return officialDocument;
    }
  }

  public static class IOfficialDocumentsExtensions
  {
    /// <summary>
    /// Обновить свойство LifeCycleState.
    /// </summary>
    /// <param name="entity">Сущность, свойство которого необходимо обновить.</param>
    /// <param name="lifeCycleState">Новое значение свойства LifeCycleState.</param>
    /// <returns>Обновленная сущность.</returns>
    public static T UpdateLifeCycleState<T>(this T entity, string lifeCycleState, bool isBatch = false) where T : IOfficialDocuments
    {
      if (!string.IsNullOrEmpty(lifeCycleState))
      {
        if (isBatch)
          return UpdateLifeCycleStateBatch(entity, lifeCycleState);

        entity = Client.Instance()
                         .For<T>()
                         .Key(entity)
                         .Set(new { LifeCycleState = lifeCycleState })
                         .UpdateEntryAsync().Result;
      }

      return entity;
    }

    private static T UpdateLifeCycleStateBatch<T>(T entity, string lifeCycleState) where T : IOfficialDocuments
    {
      BatchClient.AddRequest(odata => odata.For<T>()
                           .Key(entity)
                           .Set(new { LifeCycleState = lifeCycleState })
                           .UpdateEntryAsync());

      return entity;
    }
  }
}
