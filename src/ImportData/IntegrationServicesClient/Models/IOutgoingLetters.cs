using System;
using System.Collections.Generic;

using NLog;
using Simple.OData.Client;

namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Исходящее письмо")]
    public class IOutgoingLetters : IOfficialDocuments
    {
        public bool IsManyAddressees { get; set; }
        public IEmployees Addressee { get; set; }
        public IEmployees Assignee { get; set; }
        public IBusinessUnits BusinessUnit { get; set; }
        public ICounterparties Correspondent { get; set; }
        public IEmployees ResponsibleForReturnEmployee { get; set; }
        public IMailDeliveryMethods DeliveryMethod { get; set; }
        public IEnumerable<IOutgoingLetterAddresseess> Addressees { get; set; }

        public void CreateAddressee(IOutgoingLetterAddresseess addressee, Logger logger)
        {
            try
            {
                if (!IsManyAddressees)
                    IsManyAddressees = true;

                var result = Client.Instance().For<IOutgoingLetters>()
                 .Key(this)
                 .NavigateTo(nameof(Addressees))
                 .Set(new IOutgoingLetterAddresseess()
                 {
                     Addressee = addressee.Addressee,
                     DeliveryMethod = addressee.DeliveryMethod,
                     Correspondent = addressee.Correspondent,
                     OutgoingDocumentBase = this,
                 })
                 .InsertEntryAsync().Result;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
    }
}
