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

    public ICaseFiles CaseFile { get; set; }
    public DateTimeOffset? PlacedToCaseFileDate
    {
      get { return placedToCaseFileDate; }
      set { placedToCaseFileDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      IOfficialDocuments officialDocument = null;
      var regNumber = propertiesForSearch[Constants.KeyAttributes.CustomFieldName];

      if (GetDate(propertiesForSearch[Constants.KeyAttributes.DocumentDate], out var documentDate))
      {
        officialDocument = BusinessLogic.GetEntityWithFilter<IOfficialDocuments>(x => x.RegistrationNumber != null &&
          x.RegistrationNumber == regNumber &&
          x.DocumentDate == documentDate, exceptionList, logger);
      }

      return officialDocument;
    }

    public static IOfficialDocuments GetDocumentByRegistrationDate(IEnumerable<IOfficialDocuments> documents, DateTimeOffset regDate, Logger logger, List<Structures.ExceptionsStruct> exceptionList)
    {
      // Условие по дате регистрации не срабатывает через OData из-за ToString,
      // передача в формате даты не работает в 4.8.
      documents = documents.Where(d => d.RegistrationDate.Value.ToString("d") == regDate.ToString("d"));
      var document = documents.FirstOrDefault();

      if (documents.Count() > 1)
      {
        var message = string.Format("Найдено несколько документов с именем \"{0}\". Проверьте, что выбрана верная запись.", document.ToString());
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
        logger.Warn(message);
      }

      return document;
    }

    public static (IOfficialDocuments leadingDocument, string errorMessage) GetLeadingDocument(Logger logger, string registrationNumber, DateTimeOffset regDate, int counterpartyId = -1)
    {
      var leadingDocuments = BusinessLogic.GetEntitiesWithFilter<IContracts>(d => d.RegistrationNumber == registrationNumber &&
              d.RegistrationDate.Value.ToString("d") == regDate.ToString("d") &&
              (counterpartyId == -1 || d.Counterparty.Id == counterpartyId),
              new List<Structures.ExceptionsStruct>(), logger, true);
      // Условие по дате регистрации не срабатывает через OData из-за ToString,
      // передача в формате даты не работает в 4.8.
      leadingDocuments = leadingDocuments.Where(d => d.RegistrationDate.Value.ToString("d") == regDate.ToString("d"));

      var message = string.Empty;
      if (!leadingDocuments.Any())
        message = string.Format("Не найден ведущий документ с реквизитами \"Дата документа\" {0}, \"Рег. номер\" {1}.", regDate.ToString("d"), registrationNumber);

      if (leadingDocuments.Count() > 1)
        message = string.Format("Найдено несколько ведущих документов с реквизитами \"Дата документа\" {0}, \"Рег. номер\" {1}.", regDate.ToString("d"), registrationNumber);

      if (!string.IsNullOrEmpty(message))
      {
        logger.Error(message);
        return (null, message);
      }

      var leadingDocument = leadingDocuments.FirstOrDefault();
      return (leadingDocument, message);
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
    public static T UpdateLifeCycleState<T>(this T entity, string lifeCycleState) where T : IOfficialDocuments
    {
      if (!string.IsNullOrEmpty(lifeCycleState))
        entity = Client.Instance()
                         .For<T>()
                         .Key(entity)
                         .Set(new { LifeCycleState = lifeCycleState })
                         .UpdateEntryAsync().Result;

      return entity;
    }
  }
}
