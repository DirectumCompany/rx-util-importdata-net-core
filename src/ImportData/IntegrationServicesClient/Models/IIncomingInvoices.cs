using System;

namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Входящий счет")]
    class IIncomingInvoices : IOfficialDocuments
    {
        public string Number { get; set; }
        public ICounterparties Counterparty { get; set; }
        public DateTime? Date { get; set; }
        public double TotalAmount { get; set; }
        public ICurrency Currency { get; set; }
        public IBusinessUnits BusinessUnit { get; set; }
    }
}
