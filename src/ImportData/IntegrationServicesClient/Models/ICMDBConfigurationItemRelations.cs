using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportData.IntegrationServicesClient.Models
{
  public class ICMDBConfigurationItemRelations : IEntity
  {
    [EntityName("Связи КЕ")]

    [PropertyOptions("Тип связи между КЕ", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string RelationName { get; set; }

    [PropertyOptions("Наименование КЕ-источника", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string SourceName { get; set; }

    [PropertyOptions("Код КЕ-источника", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string SourceCode { get; set; }

    [PropertyOptions("Наименование КЕ-назначения", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string TargetName { get; set; }


    [PropertyOptions("Код КЕ-назначения", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string TargetCode { get; set; }

  }
}