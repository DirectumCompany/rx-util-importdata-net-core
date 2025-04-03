using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Подразделение")]
  public class IDepartments : IRecipients
  {
    [PropertyOptions("Телефон", RequiredType.NotRequired, PropertyType.Simple)]
    public string Phone { get; set; }

    [PropertyOptions("Краткое наименование", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string ShortName { get; set; }

    [PropertyOptions("Примечание", RequiredType.NotRequired, PropertyType.Simple)]
    public string Note { get; set; }

    [PropertyOptions("Код", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string Code { get; set; }

    [PropertyOptions("Наша организация", RequiredType.NotRequired, PropertyType.EntityWithCreate, AdditionalCharacters.ForSearch)]
    public IBusinessUnits BusinessUnit { get; set; }

    [PropertyOptions("Головное подразделение", RequiredType.NotRequired, PropertyType.EntityWithCreate)]
    public IDepartments HeadOffice { get; set; }

    [PropertyOptions("Руководитель", RequiredType.NotRequired, PropertyType.EntityWithCreate, AdditionalCharacters.ForSearch)]
    public IEmployees Manager { get; set; }

    new public static IDepartments CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.CustomFieldName];
      var businessUnitName = propertiesForSearch[Constants.KeyAttributes.BusinessUnit];
      var businessUnit = BusinessLogic.GetEntityWithFilter<IBusinessUnits>(x => x.Name == businessUnitName, exceptionList, logger);

      if (businessUnit == null)
      {
        businessUnit = BusinessLogic.CreateEntity<IBusinessUnits>(new IBusinessUnits()
        {
          Name = businessUnitName,
          Status = Constants.AttributeValue[Constants.KeyAttributes.Status]
        }, exceptionList, logger);
      }

      return BusinessLogic.CreateEntity<IDepartments>(new IDepartments()
      {
        Name = name,
        BusinessUnit = businessUnit,
        Status = Constants.AttributeValue[Constants.KeyAttributes.Status]
      }, exceptionList, logger);
    }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
        propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.Name];
      
      if (propertiesForSearch.TryGetValue(Constants.KeyAttributes.BusinessUnit, out var businessUnitName))
        return BusinessLogic.GetEntityWithFilter<IDepartments>(x => x.Name == name && (x.BusinessUnit == null || x.BusinessUnit.Name == businessUnitName), exceptionList, logger);

      return BusinessLogic.GetEntityWithFilter<IDepartments>(x => x.Name == name, exceptionList, logger);
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        return BusinessLogic.CreateEntity((IDepartments)entity, exceptionList, logger);
      else
        return BusinessLogic.UpdateEntity((IDepartments)entity, exceptionList, logger);
    }
  }
}
