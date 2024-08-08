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

    [PropertyOptions("Наша организация", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IBusinessUnits BusinessUnit { get; set; }

    [PropertyOptions("Контрагент", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public ICounterparties Counterparty { get; set; }

    [PropertyOptions("Ответственный", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IEmployees ResponsibleEmployee { get; set; }

    public IEmployees ResponsibleForReturnEmployee { get; set; }

    [PropertyOptions("Действует с", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public DateTimeOffset? ValidFrom
    {
      get { return validFrom; }
      set { validFrom = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("Действует по", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public DateTimeOffset? ValidTill
    {
      get { return validTill; }
      set { validTill = value.HasValue ? new DateTimeOffset(value.Value.Date, TimeSpan.Zero) : new DateTimeOffset?(); }
    }

    [PropertyOptions("Сумма", RequiredType.NotRequired, PropertyType.Simple)]
    public double TotalAmount { get; set; }

    [PropertyOptions("Валюта", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public ICurrencies Currency { get; set; }
  }
}
