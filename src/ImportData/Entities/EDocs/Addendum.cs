using ImportData.Entities.EDocs;
using ImportData.IntegrationServicesClient.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ImportData
{
  class Addendum : DocumentEntity
  {
    protected override bool RequiredDocumentBody { get { return true; } }
    public override int PropertiesCount { get { return 16; } }
    protected override Type EntityType { get { return typeof(IAddendums); } }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      if ((DateTimeOffset)ResultValues[Constants.KeyAttributes.RegistrationDate] == DateTimeOffset.MinValue)
        ResultValues[Constants.KeyAttributes.RegistrationDate] = null;
      
      base.FillProperies(exceptionList, logger);

      if (ResultValues[Constants.KeyAttributes.Created] == null)
        ResultValues[Constants.KeyAttributes.Created] = DateTimeOffset.Now;

      return false;
    }

    public override int RequestsPerBatch => 4;
  }
}
