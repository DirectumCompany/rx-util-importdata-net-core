using DocumentFormat.OpenXml.Wordprocessing;
using ImportData.Entities.Databooks;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Пользователи")]
  public class ISubstitutionUsers : IEntity
  {
    [PropertyOptions("НОР замещаемого", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IBusinessUnits UserBusinessUnit { get; set; }

    public IUsers User;

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
        propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.User];

      entity.ResultValues.TryGetValue(Constants.KeyAttributes.UserBusinessUnit, out var userBusinessUnit);
      var businessUnit = (IBusinessUnits)userBusinessUnit;
      if (businessUnit != null)
      {
        var employee = BusinessLogic.GetEntityWithFilter<IEmployees>(x => x.Name == name && x.Department != null && x.Department.BusinessUnit != null && x.Department.BusinessUnit.Id == businessUnit.Id, exceptionList, logger);
        if (employee != null)
          return new ISubstitutionUsers { User = employee };
      }

      var user = BusinessLogic.GetEntityWithFilter<IUsers>(x => x.Name == name, exceptionList, logger);
      if (user != null)
        return new ISubstitutionUsers { User = user };
      return null;
    }
  }
}
