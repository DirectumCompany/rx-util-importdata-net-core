using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Сотрудник")]
  public class IEmployees : IUsers
  {
    [PropertyOptions("Рабочий телефон", RequiredType.NotRequired, PropertyType.Simple)]
    public string Phone { get; set; }

    [PropertyOptions("Примечание", RequiredType.NotRequired, PropertyType.Simple)]
    public string Note { get; set; }

    [PropertyOptions("Эл.почта", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string Email { get; set; }

    public bool? NeedNotifyExpiredAssignments { get; set; }
    public bool? NeedNotifyNewAssignments { get; set; }
    public bool? NeedNotifyAssignmentsSummary { get; set; }

    [PropertyOptions("Табельный номер", RequiredType.NotRequired, PropertyType.Simple)]
    public string PersonnelNumber { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
       propertiesForSearch[Constants.KeyAttributes.CustomFieldName] :propertiesForSearch[Constants.KeyAttributes.Name];
      var email = propertiesForSearch.ContainsKey(Constants.KeyAttributes.Email) ?
        propertiesForSearch[Constants.KeyAttributes.Email] : string.Empty;

      return BusinessLogic.GetEntityWithFilter<IEmployees>(x => (email == "" && x.Name == name) || (email != "" && x.Email.ToLower().Trim() == name), exceptionList, logger);
    }

    new public static IEntity CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
       propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.Name];

      if (entity.ResultValues.TryGetValue(Constants.KeyAttributes.Person, out var person) &&
        entity.ResultValues.TryGetValue(Constants.KeyAttributes.Department, out var department))
      {
        return BusinessLogic.CreateEntity<IEmployees>(
          new IEmployees()
          {
            Name = name,
            Person = (IPersons)person,
            Department = (IDepartments)department,
            Status = Constants.AttributeValue[Constants.KeyAttributes.Status]
          },
          exceptionList,
          logger
          );
      }
      else
      {
        return BusinessLogic.GetEntityWithFilter<IEmployees>(x => x.Name == name, exceptionList, logger);
      }
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        return BusinessLogic.CreateEntity((IEmployees)entity, exceptionList, logger);
      else
        return BusinessLogic.UpdateEntity((IEmployees)entity, exceptionList, logger);
    }
  }
}
