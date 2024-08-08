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
  }
}
