using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DirRX.Test.OfficialDocument;

namespace DirRX.Test.Client
{
  partial class OfficialDocumentActions
  {
    public override void DeleteEntity(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      OfficialDocuments.Delete(_obj);
    }

    public override bool CanDeleteEntity(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}