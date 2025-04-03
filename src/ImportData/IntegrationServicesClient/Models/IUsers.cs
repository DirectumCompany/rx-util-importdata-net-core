using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Пользователи")]
  public class IUsers : IRecipients
  {
    [PropertyOptions("Логин", RequiredType.NotRequired, PropertyType.EntityWithCreate)]
    public ILogins Login { get; set; }

    [PropertyOptions("Подразделение", RequiredType.Required, PropertyType.EntityWithCreate)]
    public IDepartments Department { get; set; }

    [PropertyOptions("Персона", RequiredType.NotRequired, PropertyType.EntityWithCreate, AdditionalCharacters.CreateFromOtherProperties)]
    public IPersons Person { get; set; }

    [PropertyOptions("Должность", RequiredType.NotRequired, PropertyType.EntityWithCreate, AdditionalCharacters.CreateFromOtherProperties)]
    public IJobTitles JobTitle { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
        propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.User];

      return BusinessLogic.GetEntityWithFilter<IUsers>(x => x.Name == name, exceptionList, logger);
    }
  }
}
