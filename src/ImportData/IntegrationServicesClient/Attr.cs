using System;
using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient
{
  public class EntityName : Attribute
  {
    string name;
    public EntityName(string name)
    {
      this.name = name;
    }

    public string GetName()
    {
      return name;
    }
  }

  public class PropertyOptions : Attribute
  {
    public PropertyOptions(string excelName, RequiredType required, PropertyType type, AdditionalCharacters characters = AdditionalCharacters.Default)
    {
      ExcelName = excelName;
      Required = required;
      Type = type;
      Characters = characters;
    }

    public string ExcelName { get; }
    public RequiredType Required { get; }
    public PropertyType Type { get; }
    public AdditionalCharacters Characters { get; }

    public bool IsRequired()
    {
      return Required == RequiredType.Required;
    }

    public bool IsSimple()
    {
      return Type != PropertyType.Entity || Type != PropertyType.EntityWithCreate;
    }
  }
}
