using System;
using System.Collections.Generic;
using ImportData.IntegrationServicesClient.Models;
using NLog;

namespace ImportData
{
  class Employee : Entity
  {
    public override int PropertiesCount { get { return 23; } }
    protected override Type EntityType { get { return typeof(IEmployees); } }

    protected override string GetName()
    {
      var person = (IPersons)ResultValues[Constants.KeyAttributes.Person];

      return person.Name;
    }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.Name] = GetName();
      ResultValues[Constants.KeyAttributes.NeedNotifyExpiredAssignments] = !string.IsNullOrEmpty((string)ResultValues[Constants.KeyAttributes.Email]);
      ResultValues[Constants.KeyAttributes.NeedNotifyNewAssignments] = !string.IsNullOrEmpty((string)ResultValues[Constants.KeyAttributes.Email]);
      ResultValues[Constants.KeyAttributes.NeedNotifyAssignmentsSummary] = !string.IsNullOrEmpty((string)ResultValues[Constants.KeyAttributes.Email]);
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];
      ResultValues[Constants.KeyAttributes.EmploymentType] = BusinessLogic.GetEmploymentType((string)ResultValues[Constants.KeyAttributes.EmploymentType]);

      return false;
    }
  }
}
