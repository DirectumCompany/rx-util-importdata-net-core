using System;
using System.Collections.Generic;
using ImportData.IntegrationServicesClient.Models;
using NLog;

namespace ImportData
{
  class Employee : Person
  {
    public override int PropertiesCount { get { return 20; } }
    protected override Type EntityType { get { return typeof(IEmployees); } }

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
