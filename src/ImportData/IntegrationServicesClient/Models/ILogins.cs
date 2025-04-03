using ImportData.Entities.Databooks;
using System;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Учетная запись")]
  public class ILogins : IEntityBase
  {
    public bool? NeedChangePassword { get; set; }

    [PropertyOptions("Логин", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string LoginName { get; set; }

    public string TypeAuthentication { get; set; }
    public string Status { get; set; }

    public DateTimeOffset? PasswordLastChangeDate { get; set; }
    public DateTimeOffset? LockoutEndDate { get; set; }

    new public static IEntityBase CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var loginName = propertiesForSearch[Constants.KeyAttributes.LoginName];

      return BusinessLogic.CreateEntity(new ILogins()
      {
        LoginName = loginName,
        TypeAuthentication = Constants.AttributeValue[Constants.KeyAttributes.TypeAuthentication],
        NeedChangePassword = false,
        Status = Constants.AttributeValue[Constants.KeyAttributes.Status],
      }, exceptionList, logger);
    }

    new public static ILogins FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var loginName = propertiesForSearch[Constants.KeyAttributes.LoginName];

      return BusinessLogic.GetEntityWithFilter<ILogins>(x => x.LoginName == loginName, exceptionList, logger);
    }

    new public static IEntityBase CreateOrUpdate(IEntityBase entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        return BusinessLogic.CreateEntity((ILogins)entity, exceptionList, logger);
      else
        return BusinessLogic.UpdateEntity((ILogins)entity, exceptionList, logger);
    }
  }
}
