using System;

namespace ImportData.IntegrationServicesClient.Models
{
    public class IElectronicDocumentVersionss
    {
        private DateTimeOffset? created;

		public int Number { get; set; }
        public string Note { get; set; }
        public DateTimeOffset? Created
		{
			get { return created; }
			set { created = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
		}
		public string Modified { get; set; }
        public int Id { get; set; }
        public IBinaryData Body { get; set; }
        public IAssociatedApplications AssociatedApplication { get; set; }
    }
}
