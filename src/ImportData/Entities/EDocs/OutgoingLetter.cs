using ImportData.Entities.EDocs;
using ImportData.IntegrationServicesClient.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ImportData
{
  class OutgoingLetter : DocumentEntity
  {
    public override int PropertiesCount { get { return 12; } }
    protected override Type EntityType { get { return typeof(IOutgoingLetters); } }
    public override int RequestsPerBatch => 3;

    protected override string GetName()
    {
      var subject = ResultValues[Constants.KeyAttributes.Subject];
      var documentKind = ResultValues[Constants.KeyAttributes.DocumentKind];
      var counterparty = ResultValues[Constants.KeyAttributes.Correspondent];
      var registrationNumber = ResultValues[Constants.KeyAttributes.RegistrationNumber];
      var registrationDate = (DateTimeOffset?)ResultValues[Constants.KeyAttributes.RegistrationDate];

      return $"{documentKind} №{registrationNumber} от {registrationDate?.ToString("dd.MM.yyyy")} с {counterparty} \"{subject}\"";
    }
  }
}
