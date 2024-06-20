using System;
using System.Collections.Generic;
using System.Globalization;
using NLog;
using ImportData.IntegrationServicesClient.Models;
using System.IO;

namespace ImportData
{
  class Order : Entity
  {
    public int PropertiesCount = 14;
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
    public override IEnumerable<Structures.ExceptionsStruct> SaveToRX(Logger logger, bool supplementEntity, string ignoreDuplicates, int shift = 0)
    {
      var exceptionList = new List<Structures.ExceptionsStruct>();
      var variableForParameters = this.Parameters[shift + 0].Trim();
      var regNumber = this.Parameters[shift + 0];
      DateTimeOffset regDate = DateTimeOffset.MinValue;
      var style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
      var culture = CultureInfo.CreateSpecificCulture("en-GB");

      try
      {
        regDate = ParseDate(this.Parameters[shift + 1], style, culture);
      }
      catch (Exception)
      {
        var message = string.Format("Не удалось обработать дату регистрации \"{0}\".", this.Parameters[shift + 1]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      variableForParameters = this.Parameters[shift + 2].Trim();
      var documentKind = BusinessLogic.GetEntityWithFilter<IDocumentKinds>(d => d.Name == variableForParameters, exceptionList, logger);

      if (documentKind == null)
      {
        var message = string.Format("Не найден вид документа \"{0}\".", this.Parameters[shift + 2]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      var subject = this.Parameters[shift + 3];

      variableForParameters = this.Parameters[shift + 4].Trim();
      var businessUnit = BusinessLogic.GetEntityWithFilter<IBusinessUnits>(u => u.Name == variableForParameters, exceptionList, logger);

      if (businessUnit == null)
      {
        var message = string.Format("Не найдена НОР \"{0}\".", this.Parameters[shift + 4]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      variableForParameters = this.Parameters[shift + 5].Trim();
      IDepartments department = null;
      if (businessUnit != null)
        department = BusinessLogic.GetEntityWithFilter<IDepartments>(d => d.Name == variableForParameters && 
        (d.BusinessUnit == null || d.BusinessUnit.Id == businessUnit.Id), exceptionList, logger, true);
      else
        department = BusinessLogic.GetEntityWithFilter<IDepartments>(d => d.Name == variableForParameters, exceptionList, logger);

      if (department == null)
      {
        var message = string.Format("Не найдено подразделение \"{0}\".", this.Parameters[shift + 5]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      var filePath = this.Parameters[shift + 6];
      var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

      variableForParameters = this.Parameters[shift + 7].Trim();
      var assignee = BusinessLogic.GetEntityWithFilter<IEmployees>(e => e.Name == variableForParameters, exceptionList, logger);

      if (!string.IsNullOrEmpty(this.Parameters[shift + 7].Trim()) && assignee == null)
      {
        var message = string.Format("Не найден Исполнитель \"{2}\". Приказ: \"{0} {1}\". ", regNumber, regDate.ToString(), this.Parameters[shift + 7].Trim());
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
        logger.Warn(message);
      }

      variableForParameters = this.Parameters[shift + 8].Trim();
      var preparedBy = BusinessLogic.GetEntityWithFilter<IEmployees>(e => e.Name == variableForParameters, exceptionList, logger);

      if (!string.IsNullOrEmpty(this.Parameters[shift + 8].Trim()) && preparedBy == null)
      {
        var message = string.Format("Не найден Подготавливающий \"{2}\". Приказ: \"{0} {1}\". ", regNumber, regDate.ToString(), this.Parameters[shift + 8].Trim());
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      variableForParameters = this.Parameters[shift + 9].Trim();
      var ourSignatory = BusinessLogic.GetEntityWithFilter<IEmployees>(e => e.Name == variableForParameters, exceptionList, logger);

      if (!string.IsNullOrEmpty(this.Parameters[shift + 9].Trim()) && ourSignatory == null)
      {
        var message = string.Format("Не найден Подписывающий \"{2}\". Приказ: \"{0} {1}\". ", regNumber, regDate.ToString(), this.Parameters[shift + 9].Trim());
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
        logger.Warn(message);
      }

      var lifeCycleState = BusinessLogic.GetPropertyLifeCycleState(this.Parameters[shift + 10]);

      if (!string.IsNullOrEmpty(this.Parameters[shift + 10].Trim()) && lifeCycleState == null)
      {
        var message = string.Format("Не найдено соответствующее значение состояния \"{0}\".", this.Parameters[shift + 10]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      var note = this.Parameters[shift + 11];

			var documentRegisterIdStr = this.Parameters[shift + 12].Trim();
			if (!int.TryParse(documentRegisterIdStr, out var documentRegisterId))
				if (ExtraParameters.ContainsKey("doc_register_id"))
					int.TryParse(ExtraParameters["doc_register_id"], out documentRegisterId);

			var documentRegisters = documentRegisterId != 0 ? BusinessLogic.GetEntityWithFilter<IDocumentRegisters>(r => r.Id == documentRegisterId, exceptionList, logger) : null;

			if (documentRegisters == null)
      {
        var message = string.Format("Не найден журнал регистрации по ИД \"{0}\"", documentRegisterIdStr);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
        logger.Warn(message);

        return exceptionList;
      }

      var regState = this.Parameters[shift + 13].Trim();

      try
      {
        var isNewOrder = false;
        var orders = BusinessLogic.GetEntitiesWithFilter<IOrders>(x => x.RegistrationNumber == regNumber &&
			x.RegistrationDate.Value.ToString("d") == regDate.ToString("d") &&
			x.DocumentRegister.Id == documentRegisters.Id, exceptionList, logger, true);

        var order = (IOrders)IOfficialDocuments.GetDocumentByRegistrationDate(orders, regDate, logger, exceptionList);
        if (order == null)
        {
          order = new IOrders();
          isNewOrder = true;
        }

        order.Name = fileNameWithoutExtension;
        order.Created = DateTimeOffset.UtcNow;
        order.Name = fileNameWithoutExtension;
        order.DocumentKind = documentKind;
        order.Subject = subject;
        order.BusinessUnit = businessUnit;
        order.Department = department;
        order.Assignee = assignee;
        order.PreparedBy = preparedBy;
        order.OurSignatory = ourSignatory;
        order.Note = note;

        order.DocumentRegister = documentRegisters;
        if (regDate != DateTimeOffset.MinValue)
          order.RegistrationDate = regDate.UtcDateTime;
        else
          order.RegistrationDate = null;
        order.RegistrationNumber = regNumber;
        if (!string.IsNullOrEmpty(order.RegistrationNumber) && order.DocumentRegister != null)
          order.RegistrationState = BusinessLogic.GetRegistrationsState(regState);

        IOrders createdOrder;
        if (isNewOrder)
        {
          createdOrder = BusinessLogic.CreateEntity(order, exceptionList, logger);
					// Дополнительно обновляем свойство Состояние, так как после установки регистрационного номера Состояние сбрасывается в значение "В разработке"
					createdOrder?.UpdateLifeCycleState(lifeCycleState);
				}
        else
        {
          // Карточку не обновляем, там ошибка, если у документа есть версия.
          createdOrder = BusinessLogic.UpdateEntity(order, exceptionList, logger);
        }

        if (createdOrder == null)
          return exceptionList;

        var update_body = ExtraParameters.ContainsKey("update_body") && ExtraParameters["update_body"] == "true";
        if (!string.IsNullOrWhiteSpace(filePath))
          exceptionList.AddRange(BusinessLogic.ImportBody(createdOrder, filePath, logger, update_body));
      }
      catch (Exception ex)
      {
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = ex.Message });

        return exceptionList;
      }

      return exceptionList;
    }
  }
}
