using ImportData.IntegrationServicesClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ImportData.Entities.Databooks
{
  class ConfigurationItemLifeCycleState : Entity
  {
    public override int PropertiesCount { get { return 4; } }

    protected override Type EntityType { get { return typeof(ICMDBConfigurationItemLifeCycleStates); } }

    /// <summary>
    /// Заполнить значения свойств.
    /// </summary>
    /// <param name="exceptionList">Список ошибок и предупреждений.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Признак наличия ошибок при заполнении значений свойств.</returns>
    protected override bool FillProperties(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];
      return false;
    }

    protected override void FillCollections(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      // типов конфигурационных единиц.
      var ciLifeCycle = entity as ICMDBConfigurationItemLifeCycleStates;
      var ciItemKinds = ((string)ResultValues[Constants.KeyAttributes.AvailableFor]).Split(";");

      foreach (var ciKind in ciItemKinds)
      {
        var ciKindEntity = BusinessLogic.GetEntityWithFilter<ICMDBConfigurationItemKinds>(x => x.Name == ciKind.Trim(), exceptionList, logger);
        if (ciKindEntity != null)
          ciLifeCycle.AddConfigurationItemKinds(ciKindEntity, logger);
      }

    }
  }
}
