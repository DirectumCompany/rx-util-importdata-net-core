using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
  public class IDocflowAccountingDocumentBases : IOfficialDocuments
  {

    [PropertyOptions("Содержание", RequiredType.NotRequired, PropertyType.Simple, AdditionalCharacters.ForSearch)]
    public new string Subject { get; set; }

    [PropertyOptions("ИД журнала регистрации", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    new public IDocumentRegisters DocumentRegister { get; set; }

    [PropertyOptions("Наша организация", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IBusinessUnits BusinessUnit { get; set; }

    [PropertyOptions("Контрагент", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public ICounterparties Counterparty { get; set; }

    [PropertyOptions("Ответственный", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public IEmployees ResponsibleEmployee { get; set; }

    [PropertyOptions("№ договора", RequiredType.Required, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    new public IOfficialDocuments LeadingDocument { get; set; }

    [PropertyOptions("Сумма", RequiredType.NotRequired, PropertyType.Simple)]
    public double TotalAmount { get; set; }

    [PropertyOptions("Валюта", RequiredType.NotRequired, PropertyType.Entity, AdditionalCharacters.ForSearch)]
    public ICurrencies Currency { get; set; }

    [PropertyOptions("Номер заказа", RequiredType.NotRequired, PropertyType.Simple)]
    public string PurchaseOrderNumber { get; set; }    

  }
}
