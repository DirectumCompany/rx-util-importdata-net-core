using ImportData.IntegrationServicesClient.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.Entities.Databooks
{
  public class AssociatedApplication : Entity
  {
    public override int PropertiesCount { get { return 4; } }
    protected override Type EntityType { get { return typeof(IAssociatedApplications); } }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.OpenByDefaultForReading] = BusinessLogic.GetBoolProperty((string)ResultValues[Constants.KeyAttributes.OpenByDefaultForReading]);
      ResultValues[Constants.KeyAttributes.MonitoringType] = BusinessLogic.GetMonitoringType((string)ResultValues[Constants.KeyAttributes.MonitoringType]);
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];

      return false;
    }
  }
}
