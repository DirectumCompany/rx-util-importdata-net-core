using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
	public class IContractualDocuments : IOfficialDocuments
	{
		private DateTimeOffset? validFrom;
		private DateTimeOffset? validTill;

		public IEmployees Assignee { get; set; }
		public IBusinessUnits BusinessUnit { get; set; }
		public ICounterparties Counterparty { get; set; }
		public IEmployees ResponsibleEmployee { get; set; }
		public IEmployees ResponsibleForReturnEmployee { get; set; }
		public DateTimeOffset? ValidFrom 
		{
			get { return validFrom; }
			set { validFrom = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
		}
		public DateTimeOffset? ValidTill
		{
			get { return validTill; }
			set { validTill = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
		}
		public double TotalAmount { get; set; }
		public ICurrencies Currency { get; set; }
	}
}
