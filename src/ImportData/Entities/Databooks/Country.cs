using System;
using System.Collections.Generic;
using ImportData.IntegrationServicesClient.Models;
using NLog;

namespace ImportData
{
	class Country : Entity
	{
		public override int PropertiesCount { get { return 3; } }
    protected override Type EntityType { get { return typeof(ICountries); } }
	}
}
