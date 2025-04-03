using System.Collections.Generic;

namespace ImportData.IntegrationServicesClient.Models
{
  [EntityName("Субъект прав")]
  public class IRecipients : IEntity
  {
    public string Description { get; set; }
    public bool IsSystem { get; set; }
    public string Status { get; set; }

  }
}
