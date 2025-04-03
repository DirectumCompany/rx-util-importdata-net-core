using DocumentFormat.OpenXml.Office2010.Word;
using DocumentFormat.OpenXml.Validation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Приложение к документу")]
  public class IAddendums : IInternalDocumentBases
  {
    private DateTimeOffset? registrationDate;
    private DateTimeOffset? documentDate;

    [PropertyOptions("Рег. №", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public string RegistrationNumber { get; set; }

    [PropertyOptions("Дата регистрации", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public DateTimeOffset? RegistrationDate
    {
      get { return registrationDate; }
      set { registrationDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("№ договора", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    new public IOfficialDocuments LeadingDocument { get; set; }

    [PropertyOptions("Дата договора", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public DateTimeOffset? DocumentDate
    {
      get { return documentDate; }
      set { documentDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("Журнал регистрации", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    new public IDocumentRegisters DocumentRegister { get; set; }

    [PropertyOptions("Регистрация", RequiredType.NotRequired, PropertyType.Simple)]
    new public string RegistrationState { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      IAddendums addendum = null;
      var subject = propertiesForSearch[Constants.KeyAttributes.Subject];
      var documentKindName = propertiesForSearch[Constants.KeyAttributes.DocumentKind];
      var documentKind = BusinessLogic.GetEntityWithFilter<IDocumentKinds>(x => x.Name == documentKindName, exceptionList, logger);
      var leadingDocumentNumber = propertiesForSearch[Constants.KeyAttributes.LeadingDocument];

      if (GetDate(propertiesForSearch[Constants.KeyAttributes.DocumentDate], out var leadingDocumentDate))
      {
        var leadingDocument = BusinessLogic.GetEntityWithFilter<IOfficialDocuments>(x => x.RegistrationNumber != null &&
          x.RegistrationNumber == leadingDocumentNumber &&
          x.DocumentDate == leadingDocumentDate, exceptionList, logger);


        addendum = BusinessLogic.GetEntityWithFilter<IAddendums>(x => x.LeadingDocument.Id == leadingDocument.Id &&
          x.DocumentKind.Id == documentKind.Id &&
          x.Subject == subject, exceptionList, logger);
      }

      return addendum;
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
      {
        var lifeCycleState = ((IAddendums)entity).LifeCycleState;
        entity = BusinessLogic.CreateEntity((IAddendums)entity, exceptionList, logger, isBatch);

        return ((IAddendums)entity)?.UpdateLifeCycleState(lifeCycleState, isBatch);
      }
      else
      {
        return BusinessLogic.UpdateEntity((IAddendums)entity, exceptionList, logger);

      }
    }
  }
}
