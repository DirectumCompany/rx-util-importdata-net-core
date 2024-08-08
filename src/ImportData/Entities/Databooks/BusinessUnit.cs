using System;
using System.Collections.Generic;
using NLog;
using ImportData.IntegrationServicesClient.Models;

namespace ImportData
{
  class BusinessUnit : Entity
  {
    public override int PropertiesCount { get { return 20; } }
    protected override Type EntityType { get { return typeof(IBusinessUnits); } }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (ResultValues[Constants.KeyAttributes.HeadCompany] != null &&
         ((IBusinessUnits)ResultValues[Constants.KeyAttributes.HeadCompany]).Name == (string)ResultValues[Constants.KeyAttributes.Name])
        ResultValues[Constants.KeyAttributes.HeadCompany] = null;
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];

      return false;
    }
  }
}
