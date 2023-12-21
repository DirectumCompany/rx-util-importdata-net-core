using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Дело")]
    public class ICaseFiles : IEntity
    {
        public string Title { get; set; }
        public string Index { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Note { get; set; }
        public IDepartments Department { get; set; }
        public IFileRetentionPeriods RetentionPeriod { get; set; }
        public bool LongTerm { get; set; }
        public string Location { get; set; }
        public IRegistrationGroups RegistrationGroup { get; set; }
        public string Status { get; set; }
        public IBusinessUnits BusinessUnit { get; set; }
    }
}
