using System;
using System.Collections.Generic;
using System.Linq;
using ImportData.IntegrationServicesClient.Models;
using NLog;

namespace ImportData
{
	class Currency : Entity
	{
		public override int PropertiesCount { get { return 6; } }
    protected override Type EntityType { get { return typeof(ICurrencies); } }
	}
}
