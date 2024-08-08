using System;
using System.Collections.Generic;
using ImportData.IntegrationServicesClient.Models;
using NLog;

namespace ImportData
{
  class Department : Entity
  {
    public override int PropertiesCount { get { return 8; } }
    protected override Type EntityType { get { return typeof(IDepartments); } }

    protected override string GetName()
    {
      var manager = (IEmployees)ResultValues[Constants.KeyAttributes.Manager];
      if (manager == null)
        return string.Empty;
      return manager.Name;
    }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      var managerName = GetName();
      ResultValues[Constants.KeyAttributes.Manager] = BusinessLogic.GetEntityWithFilter<IEmployees>(x => x.Name == managerName, exceptionList, logger);

      if (ResultValues[Constants.KeyAttributes.HeadOffice] != null &&
         ((IDepartments)ResultValues[Constants.KeyAttributes.HeadOffice]).Name == (string)ResultValues[Constants.KeyAttributes.Name])
        ResultValues[Constants.KeyAttributes.HeadOffice] = null;

      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];

      return false;
    }
  }
}
