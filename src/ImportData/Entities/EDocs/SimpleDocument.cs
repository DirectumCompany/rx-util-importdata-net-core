using System;
using System.Collections.Generic;
using System.Text;
using ImportData.Entities.EDocs;
using ImportData.IntegrationServicesClient.Models;
using System.Globalization;
using NLog;
using System.Linq;

namespace ImportData
{
    class SimpleDocument : DocumentEntity
    {
        public override int PropertiesCount { get { return 13; } }
        protected override bool RequiredDocumentBody { get { return true; } }
        public override int RequestsPerBatch => 3;
        protected override Type EntityType { get { return typeof(ISimpleDocument); } }

        protected override string GetName()
        {
            var subject = ResultValues[Constants.KeyAttributes.Subject];
            var documentKind = ResultValues[Constants.KeyAttributes.DocumentKind];
            var registrationNumber = ResultValues[Constants.KeyAttributes.RegistrationNumber];
            var registrationDate = (DateTimeOffset?)ResultValues[Constants.KeyAttributes.RegistrationDate];

            return $"{documentKind} № {registrationNumber} от {registrationDate?.ToString("dd.MM.yyyy")} \"{subject}\"";
        }
        
    }
}
