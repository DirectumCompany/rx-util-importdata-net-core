using ImportData.Entities.EDocs;
using ImportData.IntegrationServicesClient.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ImportData
{
  class CompanyDirective : DocumentEntity
  {
    public override int PropertiesCount { get { return 14; } }
    protected override Type EntityType { get { return typeof(ICompanyDirective); } }
    public override int RequestsPerBatch => 4;
  }
}
