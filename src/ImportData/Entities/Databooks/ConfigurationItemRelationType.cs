using ImportData.IntegrationServicesClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportData.Entities.Databooks
{
  class ConfigurationItemRelationType : Entity
  {
    public override int PropertiesCount { get { return 5; } }
    protected override Type EntityType { get { return typeof(ICMDBConfigurationItemRelationType); } }

    protected override bool FillProperties(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];
      return false;
    }
  }
}
