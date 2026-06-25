using ImportData.IntegrationServicesClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportData.Entities.Databooks
{
  class ConfigurationItemKind : Entity
  {
    public override int PropertiesCount { get { return 4; } }
    protected override Type EntityType { get { return typeof(ICMDBConfigurationItemKinds); } }

    protected override bool FillProperties(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];
      return false;
    }
  }
}
