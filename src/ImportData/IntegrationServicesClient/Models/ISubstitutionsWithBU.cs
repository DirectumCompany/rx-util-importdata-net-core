using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Замещения")]
  public class ISubstitutionsWithBU : ISubstitutions
  {

    [PropertyOptions("НОР замещаемого", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IBusinessUnits UserBusinessUnit { get; set; }

    [PropertyOptions("НОР замещающего", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IBusinessUnits SubstituteBusinessUnit { get; set; }

    [PropertyOptions("Сотрудник", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public ISubstitutionUsers SubstitutionUser { get; set; }

    [PropertyOptions("Замещающий", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public ISubstitutionSubstitute SubstitutionSubstitute { get; set; }

    public ISubstitutions Substitution { get; set; }

    new public static IEntity CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      var userName = propertiesForSearch[Constants.KeyAttributes.User];
      var substituteName = propertiesForSearch[Constants.KeyAttributes.Substitute];

      var createdEntity = new ISubstitutionsWithBU();
      createdEntity.Substitution = BusinessLogic.CreateEntity(new ISubstitutions()
      {
        Name = string.Format("{0} - {1}", substituteName, userName),
        DelegateStrictRights = false,
        Status = Constants.AttributeValue[Constants.KeyAttributes.Status]
      }, exceptionList, logger);
      return createdEntity;
    }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (entity.ResultValues.TryGetValue(Constants.KeyAttributes.SubstitutionUser, out var user) &&
        entity.ResultValues.TryGetValue(Constants.KeyAttributes.SubstitutionSubstitute, out var substitute))
      {
        var userId = ((ISubstitutionUsers)user).User.Id;
        var substituteId = ((ISubstitutionSubstitute)substitute).User.Id;
        var substitution = BusinessLogic.GetEntityWithFilter<ISubstitutions>(x => x.User.Id == userId && x.Substitute.Id == substituteId, exceptionList, logger);

        if (substitution != null)
          return new ISubstitutionsWithBU { Substitution = substitution };
      }       

      return null;
    }
    new public static IEntityBase CreateOrUpdate(IEntity entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var substitutionsWithBU = (ISubstitutionsWithBU)entity;
      var substitute = substitutionsWithBU.SubstitutionSubstitute?.User;
      var user = substitutionsWithBU.SubstitutionUser?.User;
      if (substitutionsWithBU.Substitution == null)
      {
        substitutionsWithBU.Substitution = new ISubstitutions()
        {
          Name = string.Format("{0} - {1}", substitute?.Name, user?.Name),
          DelegateStrictRights = false,
          Status = Constants.AttributeValue[Constants.KeyAttributes.Status]
        };
      }

      substitutionsWithBU.Substitution.Substitute = substitute;
      substitutionsWithBU.Substitution.User = user;
      substitutionsWithBU.Substitution.Comment = substitutionsWithBU.Comment;
      substitutionsWithBU.Substitution.DelegateStrictRights = substitutionsWithBU.DelegateStrictRights;
      substitutionsWithBU.Substitution.StartDate = substitutionsWithBU.StartDate;
      substitutionsWithBU.Substitution.EndDate = substitutionsWithBU.EndDate;
      substitutionsWithBU.Substitution.IsSystem = substitutionsWithBU.IsSystem;

      if (isNewEntity)
        substitutionsWithBU.Substitution = BusinessLogic.CreateEntity(substitutionsWithBU.Substitution, exceptionList, logger);
      else
        substitutionsWithBU.Substitution = BusinessLogic.UpdateEntity(substitutionsWithBU.Substitution, exceptionList, logger);

      return substitutionsWithBU;
    }

  }
}
