using ImportData.IntegrationServicesClient.Models;
using NLog;
using System;
using System.Collections.Generic;

namespace ImportData.Entities.Databooks
{
  public class Login : Entity
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

      var loginName = this.Parameters[shift + 0].Trim();

      if (string.IsNullOrEmpty(loginName))
      {
        var message = string.Format("Не заполнено поле \"Логин\".");
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      var lastName = this.Parameters[shift + 1].Trim();

      if (string.IsNullOrEmpty(lastName))
      {
        var message = string.Format("Не заполнено поле \"Фамилия\".");
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      var firstName = this.Parameters[shift + 2].Trim();

      if (string.IsNullOrEmpty(firstName))
      {
        var message = string.Format("Не заполнено поле \"Имя\".");
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }
      var middleName = this.Parameters[shift + 3].Trim();
      var emplName = string.IsNullOrWhiteSpace(middleName) ? string.Format("{0} {1}", lastName, firstName) : string.Format("{0} {1} {2}", lastName, firstName, middleName);
      var employee = BusinessLogic.GetEntityWithFilter<IEmployees>(x => x.Name == emplName, exceptionList, logger);

      if (employee == null)
      {
        var message = string.Format("Не удалось найти соответствующего сотрудника \"{0} {1} {2}\".", lastName, firstName, middleName);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      try
      {

        if (ignoreDuplicates.ToLower() != Constants.ignoreDuplicates.ToLower())
        {
          var login = BusinessLogic.GetEntityWithFilter<ILogins>(x => x.LoginName == loginName, exceptionList, logger);
          // Обновление сущности при условии, что найдено одно совпадение.
          if (login == null)
          {
            login = new ILogins();
            login.LoginName = loginName;
            login.TypeAuthentication = "Windows";
            login.NeedChangePassword = false;
            login.Status = "Active";
            login = BusinessLogic.CreateEntity<ILogins>(login, exceptionList, logger);
          }

          if (login != null)
          {
              employee.Login = login;
              BusinessLogic.UpdateEntity<IEmployees>(employee, exceptionList, logger);
          }
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
