using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Наша организация")]
  public class IBusinessUnits : IRecipients
  {
    [PropertyOptions("ИНН", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string TIN { get; set; }

    [PropertyOptions("КПП", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string TRRC { get; set; }

    [PropertyOptions("Телефоны", RequiredType.NotRequired, PropertyType.Simple)]
    public string Phones { get; set; }

    [PropertyOptions("Юрид. наименование", RequiredType.NotRequired, PropertyType.Simple)]
    public string LegalName { get; set; }

    [PropertyOptions("Юридический адрес", RequiredType.NotRequired, PropertyType.Simple)]
    public string LegalAddress { get; set; }

    [PropertyOptions("Почтовый адрес", RequiredType.NotRequired, PropertyType.Simple)]
    public string PostalAddress { get; set; }

    [PropertyOptions("Примечание", RequiredType.NotRequired, PropertyType.Simple)]
    public string Note { get; set; }

    [PropertyOptions("Эл. почта", RequiredType.NotRequired, PropertyType.Simple)]
    public string Email { get; set; }

    [PropertyOptions("Сайт", RequiredType.NotRequired, PropertyType.Simple)]
    public string Homepage { get; set; }

    [PropertyOptions("", RequiredType.NotRequired, PropertyType.Simple)]
    public bool Nonresident { get; set; }

    [PropertyOptions("ОГРН", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string PSRN { get; set; }

    [PropertyOptions("ОКПО", RequiredType.NotRequired, PropertyType.Simple)]
    public string NCEO { get; set; }

    [PropertyOptions("ОКВЭД", RequiredType.NotRequired, PropertyType.Simple)]
    public string NCEA { get; set; }

    [PropertyOptions("Номер счета", RequiredType.NotRequired, PropertyType.Simple)]
    public string Account { get; set; }

    [PropertyOptions("", RequiredType.NotRequired, PropertyType.Simple)]
    public string Code { get; set; }

    [PropertyOptions("Головная орг.", RequiredType.NotRequired, PropertyType.EntityWithCreate)]
    public IBusinessUnits HeadCompany { get; set; }

    [PropertyOptions("Руководитель", RequiredType.NotRequired, PropertyType.EntityWithCreate)]
    public IEmployees CEO { get; set; }

    [PropertyOptions("Населенный пункт", RequiredType.NotRequired, PropertyType.EntityWithCreate)]
    public ICities City { get; set; }

    [PropertyOptions("Регион", RequiredType.NotRequired, PropertyType.EntityWithCreate)]
    public IRegions Region { get; set; }

    [PropertyOptions("Банк", RequiredType.NotRequired, PropertyType.EntityWithCreate)]
    public IBanks Bank { get; set; }

    new public static IBusinessUnits CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
        propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.Name];
      var manager = propertiesForSearch[Constants.KeyAttributes.CEO];
      var CEO = BusinessLogic.GetEntityWithFilter<IEmployees>(x => x.Name == manager, exceptionList, logger);

      return BusinessLogic.CreateEntity<IBusinessUnits>(new IBusinessUnits()
      {
        Name = name,
        CEO = CEO,
        Status = Constants.AttributeValue[Constants.KeyAttributes.Status]
      }, exceptionList, logger);
    }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      string name = propertiesForSearch.ContainsKey(Constants.KeyAttributes.CustomFieldName) ?
        propertiesForSearch[Constants.KeyAttributes.CustomFieldName] : propertiesForSearch[Constants.KeyAttributes.Name];


      /*
       * if (propertiesForSearch.TryGetValue(Constants.KeyAttributes.HeadCompany, out name) && !string.IsNullOrEmpty(name))
      {
        return BusinessLogic.GetEntityWithFilter<IBusinessUnits>(x => x.Name == name, exceptionList, logger);
      }

      if (propertiesForSearch.TryGetValue(Constants.KeyAttributes.BusinessUnit, out name) && !string.IsNullOrEmpty(name))
      {
        return BusinessLogic.GetEntityWithFilter<IBusinessUnits>(x => x.Name == name, exceptionList, logger);
      }

      if (propertiesForSearch.TryGetValue(Constants.KeyAttributes.Name, out name) && !string.IsNullOrEmpty(name))
      {
        return BusinessLogic.GetEntityWithFilter<IBusinessUnits>(x => x.Name == name, exceptionList, logger);
      }
      */
      return BusinessLogic.GetEntityWithFilter<IBusinessUnits>(x => x.Name == name, exceptionList, logger);
    }

    new public static void CreateOrUpdate(IEntity entity, bool isNewEntity, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (isNewEntity)
        BusinessLogic.CreateEntity((IBusinessUnits)entity, exceptionList, logger);
      else
        BusinessLogic.UpdateEntity((IBusinessUnits)entity, exceptionList, logger);
    }
  }
}
