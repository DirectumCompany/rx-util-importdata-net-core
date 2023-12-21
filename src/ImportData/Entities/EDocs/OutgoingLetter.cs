using ImportData.IntegrationServicesClient.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ImportData
{
  class OutgoingLetter : Entity
  {
    public int PropertiesCount = 12;
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
      var counterparty = BusinessLogic.GetEntityWithFilter<ICounterparties>(c => c.Name == variableForParameters, exceptionList, logger);

      if (counterparty == null)
      {
        var message = string.Format("Не найден контрагент \"{0}\".", this.Parameters[shift + 2]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      variableForParameters = this.Parameters[shift + 3].Trim();
      var documentKind = BusinessLogic.GetEntityWithFilter<IDocumentKinds>(d => d.Name == variableForParameters, exceptionList, logger);

      if (documentKind == null)
      {
        var message = string.Format("Не найден вид документа \"{0}\".", this.Parameters[shift + 3]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      var subject = this.Parameters[shift + 4];

      variableForParameters = this.Parameters[shift + 5].Trim();
      var department = BusinessLogic.GetEntityWithFilter<IDepartments>(d => d.Name == variableForParameters, exceptionList, logger);

      if (department == null)
      {
        var message = string.Format("Не найдено подразделение \"{0}\".", this.Parameters[shift + 5]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      variableForParameters = this.Parameters[shift + 6].Trim();
      var preparedBy = BusinessLogic.GetEntityWithFilter<IEmployees>(e => e.Name == variableForParameters, exceptionList, logger);

      if (!string.IsNullOrEmpty(this.Parameters[shift + 6].Trim()) && preparedBy == null)
      {
        var message = string.Format("Не найден Подготавливающий \"{2}\". Исходящее письмо: \"{0} {1}\". ", regNumber, regDate.ToString(), this.Parameters[shift + 6].Trim());
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
        logger.Warn(message);
      }

      var filePath = this.Parameters[shift + 7];
      var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

      var note = this.Parameters[shift + 8];

      variableForParameters = this.Parameters[shift + 9].Trim();
      var deliveryMethod = BusinessLogic.GetEntityWithFilter<IMailDeliveryMethods>(m => m.Name == variableForParameters, exceptionList, logger);

      if (!string.IsNullOrEmpty(this.Parameters[shift + 9].Trim()) && deliveryMethod == null)
      {
        var message = string.Format("Не найден Способ доставки \"{2}\". Входящее письмо: \"{0} {1}\". ", regNumber, regDate.ToString(), this.Parameters[shift + 9].Trim());
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
        logger.Warn(message);
      }

			var documentRegisterIdStr = this.Parameters[shift + 10].Trim();
			if (!int.TryParse(documentRegisterIdStr, out var documentRegisterId))
				if (ExtraParameters.ContainsKey("doc_register_id"))
					int.TryParse(ExtraParameters["doc_register_id"], out documentRegisterId);

			var documentRegisters = documentRegisterId != 0 ? BusinessLogic.GetEntityWithFilter<IDocumentRegisters>(r => r.Id == documentRegisterId, exceptionList, logger) : null;

			if (documentRegisters == null)
      {
        var message = string.Format("Не найден журнал регистрации по ИД \"{0}\"", documentRegisterIdStr);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      var regState = this.Parameters[shift + 11].Trim();

      try
      {
        var isNewOutgoingLetter = false;
        var regDateBeginningOfDay = BeginningOfDay(regDate.UtcDateTime);
        var outgoingLetters = BusinessLogic.GetEntitiesWithFilter<IOutgoingLetters>(x => x.RegistrationNumber == regNumber &&
			x.RegistrationDate.Value.ToString("d") == regDate.ToString("d") &&
			x.DocumentRegister.Id == documentRegisters.Id, exceptionList, logger, true);
        
        var outgoingLetter = (IOutgoingLetters)IOfficialDocuments.GetDocumentByRegistrationDate(outgoingLetters, regDate, logger, exceptionList);
        if (outgoingLetter == null)
        {
          outgoingLetter = new IOutgoingLetters();
          isNewOutgoingLetter = true;
        }

        outgoingLetter.Name = fileNameWithoutExtension;
        outgoingLetter.Correspondent = counterparty;
        outgoingLetter.Created = DateTimeOffset.UtcNow;
        outgoingLetter.DocumentKind = documentKind;
        outgoingLetter.Subject = subject;
        outgoingLetter.Department = department;
        outgoingLetter.BusinessUnit = department?.BusinessUnit;
        outgoingLetter.DeliveryMethod = deliveryMethod;
        outgoingLetter.PreparedBy = preparedBy;
        outgoingLetter.Note = note;
        outgoingLetter.DocumentRegister = documentRegisters;
        outgoingLetter.RegistrationNumber = regNumber;
        if (regDate != DateTimeOffset.MinValue)
          outgoingLetter.RegistrationDate = regDate.UtcDateTime;
        else
					outgoingLetter.RegistrationDate = null;
        if (!string.IsNullOrEmpty(outgoingLetter.RegistrationNumber) && outgoingLetter.DocumentRegister != null)
          outgoingLetter.RegistrationState = BusinessLogic.GetRegistrationsState(regState);
				IOutgoingLetters createdOutgoingLetter;
        if (isNewOutgoingLetter)
        {
          createdOutgoingLetter = BusinessLogic.CreateEntity(outgoingLetter, exceptionList, logger);
        }
        else
        {
          // Карточку не обновляем, там ошибка, если у документа есть версия.
          createdOutgoingLetter = outgoingLetter;//BusinessLogic.UpdateEntity(contract, exceptionList, logger);
        }

        var update_body = ExtraParameters.ContainsKey("update_body") && ExtraParameters["update_body"] == "true";
        if (!string.IsNullOrWhiteSpace(filePath))
          exceptionList.AddRange(BusinessLogic.ImportBody(createdOutgoingLetter, filePath, logger, update_body));
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
