using ImportData.IntegrationServicesClient.Models;
using NLog;
using System;
using System.Collections.Generic;

namespace ImportData
{
  class Company : Entity
  {
    public override int PropertiesCount { get { return 20; } }
    protected override Type EntityType { get { return typeof(ICompanies); } }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if (ResultValues[Constants.KeyAttributes.HeadCompany] != null &&
         ((IEntity)ResultValues[Constants.KeyAttributes.HeadCompany]).Name == (string)ResultValues[Constants.KeyAttributes.Name])
        ResultValues[Constants.KeyAttributes.HeadCompany] = null;
      ResultValues[Constants.KeyAttributes.Nonresident] = BusinessLogic.GetPropertyResident((string)ResultValues[Constants.KeyAttributes.Nonresident]);
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];

      return false;
    }
  }
}
