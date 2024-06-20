using System;

namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Персоны")]
    public class IPersons : ICounterparties
    {
		  private DateTimeOffset? dateOfBirth;
		  public string LastName { get; set; }
      public string FirstName { get; set; }
      public string MiddleName { get; set; }
      public DateTimeOffset? DateOfBirth
		  {
			  get { return dateOfBirth; }
			  set { dateOfBirth = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
		  }
		  public string INILA { get; set; }
      public string ShortName { get; set; }
      public string Sex { get; set; }
    }
}
