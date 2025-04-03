using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
  public class IEntity : IEntityBase
  {
    [PropertyOptions("Наименование", RequiredType.Required, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public string Name { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
