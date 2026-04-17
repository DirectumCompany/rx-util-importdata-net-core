using DocumentFormat.OpenXml.Wordprocessing;
using ImportData.Entities.Databooks;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Пользователи")]
  public class ISubstitutionSubstitute: IEntity
  {
    [PropertyOptions("НОР замещающего", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IBusinessUnits SubstituteBusinessUnit { get; set; }

    public IUsers User;

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
        propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.Substitute];

      entity.ResultValues.TryGetValue(Constants.KeyAttributes.SubstituteBusinessUnit, out var substituteBusinessUnit);
      var businessUnit = (IBusinessUnits)substituteBusinessUnit;
      if (businessUnit != null)
      {
        var employee = BusinessLogic.GetEntityWithFilter<IEmployees>(x => x.Name == name && x.Department != null && x.Department.BusinessUnit != null && x.Department.BusinessUnit.Id == businessUnit.Id, exceptionList, logger);
        if (employee != null)
          return new ISubstitutionSubstitute { User = employee };
      }

      var user = BusinessLogic.GetEntityWithFilter<IUsers>(x => x.Name == name, exceptionList, logger);
      if (user != null)
        return new ISubstitutionSubstitute { User = user };
      return null;
    }
  }
}
