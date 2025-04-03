using System;
using System.Collections.Generic;

using NLog;
using Simple.OData.Client;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Исходящее письмо")]
  public class IOutgoingLetters : IOfficialDocuments
  {
    private DateTimeOffset? registrationDate;

    [PropertyOptions("№", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public string RegistrationNumber { get; set; }

    [PropertyOptions("Дата регистрации", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public DateTimeOffset? RegistrationDate
    {
      get { return registrationDate; }
      set { registrationDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    public bool IsManyAddressees { get; set; }
    public IEmployees Addressee { get; set; }
    public IEmployees Assignee { get; set; }
    public IBusinessUnits BusinessUnit { get; set; }

    [PropertyOptions("Корреспондент", RequiredType.Required, PropertyType.EntityWithCreate, AdditionalCharacters.ForSearch)]
    public ICounterparties Correspondent { get; set; }

    public IEmployees ResponsibleForReturnEmployee { get; set; }

    [PropertyOptions("Способ доставки", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IMailDeliveryMethods DeliveryMethod { get; set; }

    public IEnumerable<IOutgoingLetterAddresseess> Addressees { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) &&
        int.TryParse(propertiesForSearch[Constants.KeyAttributes.CustomFieldName], out int OutgoingDocumentBaseId))
      {
        return BusinessLogic.GetEntityWithFilter<IOutgoingLetters>(x => x.Id == OutgoingDocumentBaseId, exceptionList, logger);
      }

      IOutgoingLetters outgoingLetters = null;
      var docRegisterId = propertiesForSearch[Constants.KeyAttributes.DocumentRegister];
      var regNumber = propertiesForSearch[Constants.KeyAttributes.RegistrationNumber];

      if (GetDate(propertiesForSearch[Constants.KeyAttributes.RegistrationDate], out var registrationDate) &&
        int.TryParse(docRegisterId, out int documentRegisterId))
      {
        outgoingLetters = BusinessLogic.GetEntityWithFilter<IOutgoingLetters>(x => x.RegistrationNumber == regNumber &&
          x.RegistrationDate == registrationDate &&
          x.DocumentRegister.Id == documentRegisterId,
        exceptionList, logger);
      }

      return outgoingLetters;
    }
    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
      {
        entity = BusinessLogic.CreateEntity((IOutgoingLetters)entity, exceptionList, logger, isBatch);
        return((IOutgoingLetters)entity)?.UpdateLifeCycleState(Constants.AttributeValue[Constants.KeyAttributes.Status], isBatch);
      }
      else
        return BusinessLogic.UpdateEntity((IOutgoingLetters)entity, exceptionList, logger);
    }

    public void CreateAddressee(IOutgoingLetterAddresseess addressee, Logger logger, bool isBatch = false)
    {
      try
      {
        if (!IsManyAddressees)
          IsManyAddressees = true;

        if (isBatch)
        {
          CreateAddresseeBatch(addressee, logger);
          return;
        }

        var result = Client.Instance().For<IOutgoingLetters>()
         .Key(this)
         .NavigateTo(nameof(Addressees))
         .Set(new IOutgoingLetterAddresseess()
         {
           Addressee = addressee.Addressee,
           DeliveryMethod = addressee.DeliveryMethod,
           Correspondent = addressee.Correspondent,
           OutgoingDocumentBase = this,
         })
         .InsertEntryAsync().Result;
      }
      catch (Exception ex)
      {
        logger.Error(ex);
        throw;
      }
    }

    private void CreateAddresseeBatch(IOutgoingLetterAddresseess addressee, Logger logger)
    {
      BatchClient.AddRequest(odata => odata.For<IOutgoingLetters>()
           .Key(this)
           .NavigateTo(nameof(Addressees))
           .Set(new IOutgoingLetterAddresseess()
           {
             Addressee = addressee.Addressee,
             DeliveryMethod = addressee.DeliveryMethod,
             Correspondent = addressee.Correspondent,
             OutgoingDocumentBase = this,
           })
           .InsertEntryAsync());
    }
  }
}
