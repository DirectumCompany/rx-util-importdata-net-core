using System;

namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Входящее письмо")]
    public class IIncomingLetters : IOfficialDocuments
    {
        private DateTimeOffset? dated;

		public DateTimeOffset? Dated
		{
			get { return dated; }
			set { dated = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
		}
		public string InNumber { get; set; }
        public bool IsManyAddressees { get; set; }
        public string ManyAddresseesPlaceholder { get; set; }
        public string ManyAddresseesLabel { get; set; }
        public IEmployees Addressee { get; set; }
        public IEmployees Assignee { get; set; }
        public IBusinessUnits BusinessUnit { get; set; }
        public ICounterparties Correspondent { get; set; }
        public IEmployees ResponsibleForReturnEmployee { get; set; }
        public IMailDeliveryMethods DeliveryMethod { get; set; }
    }
}
