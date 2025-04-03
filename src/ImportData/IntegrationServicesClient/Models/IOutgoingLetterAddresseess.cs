using ImportData.Entities.Databooks;
using System;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Список рассылки исходящего письма")]
  public class IOutgoingLetterAddresseess : IEntityBase
  {
    [PropertyOptions("Id исходящего письма в DirectumRX", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IOutgoingLetters OutgoingDocumentBase { get; set; }

    [PropertyOptions("Корреспондент", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public ICounterparties Correspondent { get; set; }

    [PropertyOptions("Адресат", RequiredType.NotRequired, PropertyType.Entity)]
    public IContacts Addressee { get; set; }

    [PropertyOptions("Способ доставки", RequiredType.NotRequired, PropertyType.Entity)]
    public IMailDeliveryMethods DeliveryMethod { get; set; }

    new public static IEntityBase FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      return null;
    }

    new public static IEntityBase CreateOrUpdate(IEntityBase entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var outgoingLetterAddresseess = (IOutgoingLetterAddresseess)entity;
      var outgoingLetter = outgoingLetterAddresseess.OutgoingDocumentBase;
      var addressee = new IOutgoingLetterAddresseess
      {
        Addressee = outgoingLetterAddresseess.Addressee,
        OutgoingDocumentBase = outgoingLetter,
        DeliveryMethod = outgoingLetterAddresseess.DeliveryMethod,
        Correspondent = outgoingLetterAddresseess.Correspondent,
      };
      outgoingLetter.CreateAddressee(addressee, logger, isBatch);
      return addressee;
    }
  }
}
