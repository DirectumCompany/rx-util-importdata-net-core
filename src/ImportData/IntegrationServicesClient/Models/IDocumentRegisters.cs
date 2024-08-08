using System.Collections.Generic;
using System;
using System.Linq;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Журналы регистрации")]
  public class IDocumentRegisters : IEntity
  {
    public string Index { get; set; }
    public int NumberOfDigitsInNumber { get; set; }
    public string ValueExample { get; set; }
    public string DisplayName { get; set; }
    public string DocumentFlow { get; set; }
    public string RegisterType { get; set; }
    public string NumberingPeriod { get; set; }
    public string NumberingSection { get; set; }
    public string Status { get; set; }
    public IRegistrationGroups RegistrationGroup { get; set; }

    new public static IEntity FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (int.TryParse(propertiesForSearch[Constants.KeyAttributes.CustomFieldName], out int documentRegisterId))
        return BusinessLogic.GetEntityWithFilter<IDocumentRegisters>(x => x.Id == documentRegisterId, exceptionList, logger);

      return null;
    }
  }
}
