using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
  public class ICompanyDirective : IInternalDocumentBases
  {
    private DateTimeOffset? registrationDate;

    [PropertyOptions("Дата", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    new public DateTimeOffset? RegistrationDate
    {
      get { return registrationDate; }
      set { registrationDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("Подробности", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    new public IDocumentKinds DocumentKind { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var documentKindName = propertiesForSearch[Constants.KeyAttributes.DocumentKind];
      var subject = propertiesForSearch[Constants.KeyAttributes.Subject];
      var registrationNumber = propertiesForSearch[Constants.KeyAttributes.RegistrationNumber];

      if (GetDate(propertiesForSearch[Constants.KeyAttributes.RegistrationDate], out var registrationDate))
      {
        var name = $"{documentKindName} №{registrationNumber} от {registrationDate.ToString("dd.MM.yyyy")} \"{subject}\"";
        return BusinessLogic.GetEntityWithFilter<ICompanyDirective>(x => x.Name == name, exceptionList, logger);
      }

      return null;
    }

    new public static IEntity CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
      {
        var lifeCycleState = ((ICompanyDirective)entity).LifeCycleState;
        entity = BusinessLogic.CreateEntity((ICompanyDirective)entity, exceptionList, logger, isBatch);
        return ((ICompanyDirective)entity)?.UpdateLifeCycleState(lifeCycleState, isBatch);
      }
      else
      {
        return BusinessLogic.UpdateEntity((ICompanyDirective)entity, exceptionList, logger);
      }
    }
  }
}
