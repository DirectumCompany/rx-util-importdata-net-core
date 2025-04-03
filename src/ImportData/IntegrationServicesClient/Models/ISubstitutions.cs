using System;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Замещения")]
  public class ISubstitutions : IEntity
  {
    private DateTimeOffset? startDate;
    private DateTimeOffset? endDate;

    [PropertyOptions("Дата начала", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public DateTimeOffset? StartDate
    {
      get { return startDate; }
      set { startDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("Дата завершения", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public DateTimeOffset? EndDate
    {
      get { return endDate; }
      set { endDate = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    public bool IsSystem { get; set; }
    public bool DelegateStrictRights { get; set; }
    public string Comment { get; set; }
    public string Status { get; set; }

    [PropertyOptions("Сотрудник", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IUsers User { get; set; }

    [PropertyOptions("Замещающий", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IUsers Substitute { get; set; }

    new public static IEntity CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var userName = propertiesForSearch[Constants.KeyAttributes.User];
      var substituteName = propertiesForSearch[Constants.KeyAttributes.Substitute];

      return BusinessLogic.CreateEntity(new ISubstitutions()
      {
        Name = string.Format("{0} - {1}", substituteName, userName),
        DelegateStrictRights = false,
        Status = Constants.AttributeValue[Constants.KeyAttributes.Status]
      }, exceptionList, logger);
    }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var userName = propertiesForSearch[Constants.KeyAttributes.User];
      var substituteName = propertiesForSearch[Constants.KeyAttributes.Substitute];
      var name = string.Format("{0} - {1}", substituteName, userName);

      return BusinessLogic.GetEntityWithFilter<ISubstitutions>(x => x.User.Name == name && x.Substitute.Name == substituteName, exceptionList, logger);
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        return BusinessLogic.CreateEntity((ISubstitutions)entity, exceptionList, logger);
      else
        return BusinessLogic.UpdateEntity((ISubstitutions)entity, exceptionList, logger);
    }
  }
}
