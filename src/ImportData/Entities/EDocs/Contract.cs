using System;
using System.Collections.Generic;
using System.Globalization;
using NLog;
using ImportData.IntegrationServicesClient.Models;
using System.IO;
using DocumentFormat.OpenXml.Drawing.Charts;
using ImportData.Entities.Databooks;
using System.Linq;
using ImportData.Entities.EDocs;

namespace ImportData
{
  public class Contract : DocumentEntity
  {
    protected override bool RequiredDocumentBody { get { return true; } }
    public override int PropertiesCount { get { return 21; } }
    protected override Type EntityType { get { return typeof(IContracts); } }
    public override int RequestsPerBatch => 4;

    protected override string GetName()
    {
      var subject = ResultValues[Constants.KeyAttributes.Subject];
      var documentKind = ResultValues[Constants.KeyAttributes.DocumentKind];
      var counterparty = ResultValues[Constants.KeyAttributes.Counterparty];
      var registrationNumber = ResultValues[Constants.KeyAttributes.RegistrationNumber];
      var registrationDate = (DateTimeOffset?)ResultValues[Constants.KeyAttributes.RegistrationDate];

      return $"{documentKind} №{registrationNumber} от {registrationDate?.ToString("dd.MM.yyyy")} с {counterparty} \"{subject}\"";
    }
  }
}
