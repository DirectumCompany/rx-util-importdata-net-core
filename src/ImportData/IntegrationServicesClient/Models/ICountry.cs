﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
	[EntityName("Страны")]
	public class ICountry : IEntity
	{
		public string Status { get; set; }
		public string Code { get; set; }
	}
}
