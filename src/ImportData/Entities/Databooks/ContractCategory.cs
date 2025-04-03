using DocumentFormat.OpenXml.InkML;
using ImportData.IntegrationServicesClient.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.Entities.Databooks
{
  public class ContractCategory : Entity
  {
    public override int PropertiesCount { get { return 3; } }
    protected override Type EntityType { get { return typeof(IContractCategories); } }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];
      return false;
    }

    protected override void FillCollections(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      // Импорт видов документов.
      var category = entity as IContractCategories;
      var documentKinds = ((string)ResultValues[Constants.KeyAttributes.DocumentKinds]).Split(";");
      foreach (var documentKind in documentKinds)
      {
        var documentKindEntity = BusinessLogic.GetEntityWithFilter<IDocumentKinds>(x => x.Name == documentKind.Trim(), exceptionList, logger);
        if (documentKindEntity != null)
          category.AddDocumentKind(documentKindEntity, logger);
      }
    }
  }
}
