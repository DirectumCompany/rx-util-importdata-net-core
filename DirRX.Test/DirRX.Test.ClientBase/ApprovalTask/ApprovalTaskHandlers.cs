using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DirRX.Test.ApprovalTask;

namespace DirRX.Test
{
  partial class ApprovalTaskClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      Sungero.Workflow.Tasks.As(_obj).State.Panels.AttachmentsPreview.IsVisible = false;
    }

  }
}