using ImportData.IntegrationServicesClient.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.Entities.Databooks
{
  public class DocumentKind : Entity
  {
    public override int PropertiesCount { get { return 9; } }
    protected override Type EntityType { get { return typeof(IDocumentKinds); } }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.DocumentFlow] = BusinessLogic.GetDocumentFlow((string)ResultValues[Constants.KeyAttributes.DocumentFlow]);
      ResultValues[Constants.KeyAttributes.NumberingType] = BusinessLogic.GetNumberingType((string)ResultValues[Constants.KeyAttributes.NumberingType]);
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];

      int value;
      if (int.TryParse((string)ResultValues[Constants.KeyAttributes.DeadlineInDays], out value))
        ResultValues[Constants.KeyAttributes.DeadlineInDays] = value;
      else
        ResultValues[Constants.KeyAttributes.DeadlineInDays] = null;

      if (int.TryParse((string)ResultValues[Constants.KeyAttributes.DeadlineInHours], out value))
        ResultValues[Constants.KeyAttributes.DeadlineInHours] = value;
      else
        ResultValues[Constants.KeyAttributes.DeadlineInHours] = null;

      return false;
    }
  }
}
