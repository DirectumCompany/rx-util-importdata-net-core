using ImportData.IntegrationServicesClient.Models;
using NLog;
using System;
using System.Collections.Generic;

namespace ImportData.Entities.Databooks
{
  public class Login : Entity
  {
    public override int PropertiesCount { get { return 4; } }
    protected override Type EntityType { get { return typeof(ILogins); } }

    public override IEnumerable<Structures.ExceptionsStruct> SaveToRX(NLog.Logger logger, string ignoreDuplicates)
    {
      var exceptionList = new List<Structures.ExceptionsStruct>();

      // Проверим наличие полей в таблице, получим сотрудника в системе, чтобы после импорта логина добавить логин к сотруднику.
      if (CheckEmployee(EntityType, logger, out var exceptions, out var employee))
      {
        if (exceptions.Count > 0)
        {
          exceptionList.AddRange(exceptions);
          return exceptionList;
        }
      }
      else
      {
        exceptionList.AddRange(exceptions);
        return exceptionList;
      }
      // Импорт логинов.
      exceptionList.AddRange(base.SaveToRX(logger, ignoreDuplicates));
      
      // Добавляем информацию по логину для сотрудника и обновляем данные в системе.
      employee.Login = (ILogins)entity;
      MethodCall(employee.GetType(), Constants.EntityActions.CreateOrUpdate, employee, false, exceptionList, logger);

      return exceptionList;
    }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.NeedChangePassword] = false;
      ResultValues[Constants.KeyAttributes.TypeAuthentication] = Constants.AttributeValue[Constants.KeyAttributes.TypeAuthentication];
      ResultValues[Constants.KeyAttributes.Status] = Constants.AttributeValue[Constants.KeyAttributes.Status];

      return false;
    }

    /// <summary>
    /// Проверка обязательных для заполнения параметров и поиск сущности.
    /// </summary>
    /// <param name="entityType">Сущность RX для заполнения.</param>
    /// <returns>Результат проверки.</returns>
    private bool CheckEmployee(Type entityType, Logger logger, out List<Structures.ExceptionsStruct> exceptionList, out IEmployees employee)
    {
      employee = null;
      exceptionList = new List<Structures.ExceptionsStruct>();
      if (!(NamingParameters.ContainsKey(Constants.KeyAttributes.FirstNameRu) &&
        NamingParameters.ContainsKey(Constants.KeyAttributes.LastNameRu) &&
        NamingParameters.ContainsKey(Constants.KeyAttributes.MiddleNameRu)))
      {
        return false;
      }

      var firstName = NamingParameters[Constants.KeyAttributes.FirstNameRu];
      var middleName = NamingParameters[Constants.KeyAttributes.MiddleNameRu];
      var lastName = NamingParameters[Constants.KeyAttributes.LastNameRu];

      if (string.IsNullOrWhiteSpace(firstName))
      {
        exceptionList.Add(new Structures.ExceptionsStruct
        {
          ErrorType = Constants.ErrorTypes.Error,
          Message = string.Format(Constants.Resources.EmptyColumn, Constants.KeyAttributes.FirstNameRu)
        });
        return false;
      }
      if (string.IsNullOrWhiteSpace(lastName))
      {
        exceptionList.Add(new Structures.ExceptionsStruct
        {
          ErrorType = Constants.ErrorTypes.Error,
          Message = string.Format(Constants.Resources.EmptyColumn, Constants.KeyAttributes.LastNameRu)
        });
        return false;
      }

      var name = string.IsNullOrWhiteSpace(middleName) ? string.Format("{0} {1}", lastName, firstName) : string.Format("{0} {1} {2}", lastName, firstName, middleName);
      employee = BusinessLogic.GetEntityWithFilter<IEmployees>(x => x.Name == name, exceptionList, logger);

      if (employee == null)
      {
        exceptionList.Add(new Structures.ExceptionsStruct
        {
          ErrorType = Constants.ErrorTypes.Error,
          Message = string.Format(Constants.Resources.ErrorFindEmployee, name)
        });
        return false;
      }

      return true;
    }
  }
}
