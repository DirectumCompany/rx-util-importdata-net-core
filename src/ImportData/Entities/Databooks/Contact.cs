using System;
using System.Collections.Generic;
using NLog;
using ImportData.IntegrationServicesClient.Models;

namespace ImportData.Entities.Databooks
{
  public class Contact : Entity
  {
    public override int PropertiesCount { get { return 10; } }
    protected override Type EntityType { get { return typeof(IContacts); } }

    protected override string GetName()
    {
      var person = (IPersons)ResultValues[Constants.KeyAttributes.Person];
      return person.Name;
    }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.Name] = GetName();
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];

      return false;
    }
  }
}