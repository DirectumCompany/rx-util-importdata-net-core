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
      {
        ResultValues[Constants.KeyAttributes.HeadCompany] = null;
      }

      ResultValues[Constants.KeyAttributes.Nonresident] = BusinessLogic.GetBoolProperty((string)ResultValues[Constants.KeyAttributes.Nonresident]);
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];

      var nonresident = (bool)ResultValues[Constants.KeyAttributes.Nonresident];
      var tin = (string)ResultValues[Constants.KeyAttributes.TIN];
      var trrc = (string)ResultValues[Constants.KeyAttributes.TRRC];
      var psrn = (string)ResultValues[Constants.KeyAttributes.PSRN];
      var nceo = (string)ResultValues[Constants.KeyAttributes.NCEO];

      return CheckCompanyRequsite(nonresident, tin, trrc, psrn, nceo, exceptionList, logger);
    }
  }
}
