using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Срок хранения дела")]
    public class IFileRetentionPeriods : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? RetentionPeriod { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
    }
}
