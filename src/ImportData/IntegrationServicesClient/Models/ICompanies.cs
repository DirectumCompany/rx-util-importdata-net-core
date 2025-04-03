using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Организация")]
  public class ICompanies : ICounterparties
  {
    [PropertyOptions("КПП", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string TRRC { get; set; }

    public bool IsCardReadOnly { get; set; }

    [PropertyOptions("Юрид. наименование", RequiredType.NotRequired, PropertyType.Simple)]
    public string LegalName { get; set; }

    [PropertyOptions("Юридический адрес", RequiredType.NotRequired, PropertyType.Simple)]
    new public string LegalAddress { get; set; }

    [PropertyOptions("Головная орг.", RequiredType.NotRequired, PropertyType.EntityWithCreate)]
    public ICompanies HeadCompany { get; set; }

    new public static ICompanies FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = string.Empty;

      // Если используется кастомный реквизит, то поиск может выполняться только по имени, т.к. при импорте организаций и подразделений головной организации и НОР соответственно
      // прочих реквизитов (ИНН, КПП, ОГРН, ОКПО) в шаблоне нет. Иначе ищем в том числе по реквизитам.
      if (propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName))
      {
        name = propertiesForSearch[Constants.KeyAttributes.CustomFieldName];
        return BusinessLogic.GetEntityWithFilter<ICompanies>(x => x.Name == name, exceptionList, logger);
      }

      name = propertiesForSearch[Constants.KeyAttributes.Name];
      var tin = (string)propertiesForSearch[Constants.KeyAttributes.TIN];
      var trrc = (string)propertiesForSearch[Constants.KeyAttributes.TRRC];
      var psrn = (string)propertiesForSearch[Constants.KeyAttributes.PSRN];
      var nceo = (string)propertiesForSearch[Constants.KeyAttributes.NCEO];

      return BusinessLogic.GetEntityWithFilter<ICompanies>(x => x.Name == name ||
        (tin != string.Empty && x.TIN == tin && trrc != string.Empty && x.TRRC == trrc) ||
        (psrn != string.Empty && x.PSRN == psrn),
        exceptionList, logger);

    }

    new public static ICompanies CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var name = propertiesForSearch[Constants.KeyAttributes.Name];

      return BusinessLogic.CreateEntity<ICompanies>(new ICompanies()
      {
        Name = name,
        Status = Constants.AttributeValue[Constants.KeyAttributes.Status]
      }, exceptionList, logger);
    }

    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        return BusinessLogic.CreateEntity((ICompanies)entity, exceptionList, logger);
      else
        return BusinessLogic.UpdateEntity((ICompanies)entity, exceptionList, logger);
    }
  }
}
