namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Сотрудник")]
    public class IEmployees : IUsers
    {
        public string Phone { get; set; }
        public string Note { get; set; }
        public string Email { get; set; }
        public bool? NeedNotifyExpiredAssignments { get; set; }
        public bool? NeedNotifyNewAssignments { get; set; }
        public bool? NeedNotifyAssignmentsSummary { get; set; }
        public string PersonnelNumber { get; set; }
    }
}
