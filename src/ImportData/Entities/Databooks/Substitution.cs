using System;
using System.Collections.Generic;
using NLog;
using ImportData.IntegrationServicesClient.Models;

namespace ImportData.Entities.Databooks
{
  public class Substitution : Entity
  {
    public override int PropertiesCount { get { return 4; } }
    protected override Type EntityType { get { return typeof(ISubstitutions); } }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];

      if (ResultValues[Constants.KeyAttributes.StartDate] != null && (DateTimeOffset)ResultValues[Constants.KeyAttributes.StartDate] == DateTimeOffset.MinValue)
        ResultValues[Constants.KeyAttributes.StartDate] = null;

      if (ResultValues[Constants.KeyAttributes.EndDate] != null && (DateTimeOffset)ResultValues[Constants.KeyAttributes.EndDate] == DateTimeOffset.MinValue)
        ResultValues[Constants.KeyAttributes.EndDate] = null;

      return false;
    }
  }
}
