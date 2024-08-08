using System;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Персоны")]
  public class IPersons : ICounterparties
  {
    private DateTimeOffset? dateOfBirth;

    [PropertyOptions("Фамилия", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string LastName { get; set; }

    [PropertyOptions("Имя", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string FirstName { get; set; }

    [PropertyOptions("Отчество", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string MiddleName { get; set; }

    [PropertyOptions("Дата рождения", RequiredType.NotRequired, PropertyType.Simple)]
    public DateTimeOffset? DateOfBirth
    {
      get { return dateOfBirth; }
      set { dateOfBirth = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("СНИЛС", RequiredType.NotRequired, PropertyType.Simple)]
    public string INILA { get; set; }

    public string ShortName { get; set; }

    [PropertyOptions("Пол", RequiredType.NotRequired, PropertyType.Simple)]
    public string Sex { get; set; }

    new public static IEntity CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var firstName = propertiesForSearch[Constants.KeyAttributes.FirstName];
      var middleName = propertiesForSearch[Constants.KeyAttributes.MiddleName];
      var lastName = propertiesForSearch[Constants.KeyAttributes.LastName];
      var person = BusinessLogic.GetEntityWithFilter<IPersons>(x => x.FirstName == firstName && x.MiddleName == middleName && x.LastName == lastName, exceptionList, logger);

      if (person != null)
        return person;

      return BusinessLogic.CreateEntity(new IPersons()
      {
        FirstName = firstName,
        MiddleName = middleName,
        LastName = lastName,
        Name = string.Format("{0} {1} {2}", lastName, firstName, middleName),
        Status = Constants.AttributeValue[Constants.KeyAttributes.Status]
      }, exceptionList, logger);
    }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var firstName = propertiesForSearch[Constants.KeyAttributes.FirstName];
      var middleName = propertiesForSearch[Constants.KeyAttributes.MiddleName];
      var lastName = propertiesForSearch[Constants.KeyAttributes.LastName];

      return BusinessLogic.GetEntityWithFilter<IPersons>(x => x.FirstName == firstName && x.MiddleName == middleName && x.LastName == lastName, exceptionList, logger);
    }

    new public static void CreateOrUpdate(IEntity entity, bool isNewEntity, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        BusinessLogic.CreateEntity((IPersons)entity, exceptionList, logger);
      else
        BusinessLogic.UpdateEntity((IPersons)entity, exceptionList, logger);
    }
  }
}
