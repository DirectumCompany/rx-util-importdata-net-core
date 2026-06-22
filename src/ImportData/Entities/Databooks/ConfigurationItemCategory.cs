using ImportData.IntegrationServicesClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ImportData.Entities.Databooks
{
  class ConfigurationItemCategory : Entity
  {
    public override int PropertiesCount { get { return 1; } }

    protected override Type EntityType { get { return typeof(ICMDBConfigurationItemCategories); } }

    protected override bool FillProperties(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];
      ResultValues[Constants.KeyAttributes.ConfigurationItemCategoryGuid] = Guid.NewGuid().ToString();
      return false;
    }
  }
}
