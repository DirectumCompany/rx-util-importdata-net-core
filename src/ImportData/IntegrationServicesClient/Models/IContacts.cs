using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
    [EntityName("Контакт")]
    public class IContacts : IEntity
    {
        public string JobTitle { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public string Homepage { get; set; }
        public string Status { get; set; }
        public ICompanies Company { get; set; }
        public IPersons Person { get; set; }
    }
}
