using System;
using System.Collections.Generic;
using NLog;
using ImportData.IntegrationServicesClient.Models;

namespace ImportData.Entities.Databooks
{
  public class Substitution : Entity
  {
    public int PropertiesCount = 4;

    /// <summary>
    /// Получить наименование число запрашиваемых параметров.
    /// </summary>
    /// <returns>Число запрашиваемых параметров.</returns>
    public override int GetPropertiesCount()
    {
      return PropertiesCount;
    }

    /// <summary>
    /// Сохранение сущности в RX.
    /// </summary>
    /// <param name="shift">Сдвиг по горизонтали в XLSX документе. Необходим для обработки документов, составленных из элементов разных сущностей.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Число запрашиваемых параметров.</returns>
    public override IEnumerable<Structures.ExceptionsStruct> SaveToRX(Logger logger, string ignoreDuplicates, int shift = 0)
    {
      var exceptionList = new List<Structures.ExceptionsStruct>();

      var substitute = this.Parameters[shift + 0].Trim();

      if (string.IsNullOrEmpty(substitute))
      {
        var message = string.Format("Не заполнено поле \"Замещаюший\".");
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      var substUser = this.Parameters[shift + 1].Trim();

      if (string.IsNullOrEmpty(substUser))
      {
        var message = string.Format("Не заполнено поле \"Сотрудник\".");
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      var substituteRecipient = BusinessLogic.GetEntityWithFilter<IUsers>(x => x.Name == substitute, exceptionList, logger);
      if (substituteRecipient == null)
      {
        var message = string.Format("Не удалось найти соответствующего сотрудника \"{0}\".", substitute);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      var substUserRecipient = BusinessLogic.GetEntityWithFilter<IUsers>(x => x.Name == substUser, exceptionList, logger);
      if (substituteRecipient == null)
      {
        var message = string.Format("Не удалось найти соответствующего сотрудника \"{0}\".", substUser);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      try
      {

        var substitution = BusinessLogic.GetEntityWithFilter<ISubstitutions>(x => x.Substitute == substituteRecipient && x.User == substUserRecipient, exceptionList, logger);
        // Обновление сущности при условии, что найдено одно совпадение.
        if (substitution == null)
        {
          substitution = new ISubstitutions();
          substitution.Substitute = substituteRecipient;
          substitution.User = substUserRecipient;
          substitution.IsSystem = false;
          substitution.StartDate = null;
          substitution.EndDate = null;
          substitution.Status = "Active";
          BusinessLogic.CreateEntity<ISubstitutions>(substitution, exceptionList, logger);
          return exceptionList;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = ex.Message });

        return exceptionList;
      }

      return exceptionList;
    }
  }
}
