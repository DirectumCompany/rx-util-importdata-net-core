using ImportData.IntegrationServicesClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportData.Entities.Databooks
{
	public class ConfigurationItem : Entity
	{
		public override int PropertiesCount { get { return 11; } }
		protected override Type EntityType { get { return typeof(ICMDBConfigurationItems); } }
	}
}
