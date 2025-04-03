using System;
using System.Collections.Generic;
using NLog;
using ImportData.IntegrationServicesClient.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ImportData.Entities.Databooks
{
  internal class CaseFile : Entity
  {
    public override int PropertiesCount { get { return 10; } }
    protected override Type EntityType { get { return typeof(ICaseFiles); } }
    protected override string GetName()
    {
      var title = ResultValues[Constants.KeyAttributes.Title];
      var index = ResultValues[Constants.KeyAttributes.Index];

      return string.Format("{0}. {1}", index, title);
    }

    private DateTimeOffset? GetDateTime(string name)
    {
      if (ResultValues[name] != null)
      {
        var date = (DateTimeOffset)ResultValues[name];
        if (date == DateTimeOffset.MinValue)
          return null;

        return date;
      }
      else
      {
        return null;
      }
    }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.Name] = GetName();
      ResultValues[Constants.KeyAttributes.StartDate] = GetDateTime(Constants.KeyAttributes.StartDate);
      ResultValues[Constants.KeyAttributes.EndDate] = GetDateTime(Constants.KeyAttributes.EndDate);
      ResultValues[Constants.KeyAttributes.LongTerm] = false;
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];

      return false;
    }
  }
}
