using System;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Замещения")]
  public class ISubstitutions : IEntity
  {
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsSystem { get; set; }
    public string Comment { get; set; }
    public string Status { get; set; }
    public IUsers User { get; set; }
    public IUsers Substitute { get; set; }
  }
}
