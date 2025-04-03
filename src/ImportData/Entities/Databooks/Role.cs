using ImportData.IntegrationServicesClient.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.Entities.Databooks
{
  public class Role : Entity
  {
    public override int PropertiesCount { get { return 3; } }
    protected override Type EntityType { get { return typeof(IRoles); } }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];
      return false;
    }

    protected override void FillCollections(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      // Импорт состава роли.
      var role = entity as IRoles;
      var recipients = ((string)ResultValues[Constants.KeyAttributes.RecipientLinks]).Split(";");
      foreach (var recipient in recipients)
      {
        var recipientEntity = BusinessLogic.GetEntityWithFilter<IRecipients>(x => x.Name == recipient.Trim(), exceptionList, logger);
        if (recipientEntity != null)
          role.AddRecipient(recipientEntity, logger);
      }
    }
  }
}
