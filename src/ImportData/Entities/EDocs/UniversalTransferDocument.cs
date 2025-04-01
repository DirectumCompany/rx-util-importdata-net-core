using ImportData.Entities.EDocs;
using ImportData.IntegrationServicesClient.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.Entities.EDocs
{
  public class UniversalTransferDocument : DocumentEntity
  {    
    protected override bool RequiredDocumentBody { get { return true; } }
    public override int PropertiesCount { get { return 20; } }
    protected override Type EntityType { get { return typeof(IUniversalTransferDocuments); } }
    public override int RequestsPerBatch => 4;

    protected override string GetName()
    {
      var subject = ResultValues[Constants.KeyAttributes.Subject];
      var documentKind = ResultValues[Constants.KeyAttributes.DocumentKind];
      var counterparty = ResultValues[Constants.KeyAttributes.Counterparty];
      var registrationNumber = ResultValues[Constants.KeyAttributes.RegistrationNumber];
      var registrationDate = (DateTimeOffset?)ResultValues[Constants.KeyAttributes.RegistrationDate];

      return $"{documentKind} №{registrationNumber} от {registrationDate?.ToString("dd.MM.yyyy")} {counterparty} \"{subject}\"";
    }
  }
}
