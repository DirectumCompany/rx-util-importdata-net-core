﻿using System;
using System.Collections.Generic;
using NLog;
using ImportData.IntegrationServicesClient.Models;
using ImportData.Entities.EDocs;

namespace ImportData
{
  class OutgoingLetterAddressees : DocumentEntity
  {
    public override int PropertiesCount { get { return 4; } }
    protected override Type EntityType { get { return typeof(IOutgoingLetterAddresseess); } }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, Logger logger)
    {
      ResultValues[Constants.KeyAttributes.ManyAddresses] = true;

      return false;
    }
  }
}
