using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DirRX.Test.FreeApprovalTask;

namespace DirRX.Test
{
  partial class FreeApprovalTaskClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      _obj.State.Panels.AttachmentsPreview.IsVisible = false;
      _obj.State.Panels.AccessRights.IsVisible = false;
      _obj.State.Panels.Attachments.IsVisible = false;
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      _obj.State.Panels.AttachmentsPreview.IsVisible = false;
      //Sungero.Workflow.Tasks.As(_obj).State.Panels.AttachmentsPreview.IsVisible = false;
    }

  }
}