using System;
using System.Collections.Generic;
using System.Globalization;
using NLog;
using ImportData.IntegrationServicesClient.Models;
using System.Linq.Expressions;

namespace ImportData
{
  class Person : Entity
  {
    public override int PropertiesCount { get { return 17; } }
    protected override Type EntityType { get { return typeof(IPersons); } }

    protected override string GetName()
    {
      var firstName = ResultValues[Constants.KeyAttributes.FirstName];
      var middleName = ResultValues[Constants.KeyAttributes.MiddleName];
      var lastName = ResultValues[Constants.KeyAttributes.LastName];

      return string.Format("{0} {1} {2}", lastName, firstName, middleName);
    }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.Name] = GetName();
      ResultValues[Constants.KeyAttributes.Sex] = BusinessLogic.GetPropertySex((string)ResultValues[Constants.KeyAttributes.Sex]);
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];

      if (ResultValues[Constants.KeyAttributes.DateOfBirth] != null &&
        (DateTimeOffset)ResultValues[Constants.KeyAttributes.DateOfBirth] == DateTimeOffset.MinValue)
      {
        ResultValues[Constants.KeyAttributes.DateOfBirth] = null;
      }

      return false;
    }
  }
}
