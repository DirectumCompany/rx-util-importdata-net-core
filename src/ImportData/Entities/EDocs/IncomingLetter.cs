using System;
using System.Collections.Generic;
using System.Globalization;
using NLog;
using ImportData.IntegrationServicesClient.Models;
using System.IO;

namespace ImportData
{
  class IncomingLetter : Entity
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
      var counterparty = BusinessLogic.GetEntityWithFilter<ICounterparties>(c => c.Name == variableForParameters, exceptionList, logger);

      if (counterparty == null)
      {
        var message = string.Format("Не найден контрагент \"{0}\".", this.Parameters[shift + 2]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      variableForParameters = this.Parameters[shift + 3].Trim();
      var documentKind = BusinessLogic.GetEntityWithFilter<IDocumentKinds>(k => k.Name == variableForParameters, exceptionList, logger);

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

      var filePath = this.Parameters[shift + 6];
      var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

      var dated = DateTimeOffset.MinValue;
      try
      {
        dated = ParseDate(this.Parameters[shift + 7], style, culture);
      }
      catch (Exception)
      {
        var message = string.Format("Не удалось обработать значение в поле \"Письмо от\" \"{0}\".", this.Parameters[shift + 7]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      var inNumber = this.Parameters[shift + 8];

      variableForParameters = this.Parameters[shift + 9].Trim();
      var addressee = BusinessLogic.GetEntityWithFilter<IEmployees>(e => e.Name == variableForParameters, exceptionList, logger);

      if (!string.IsNullOrEmpty(this.Parameters[shift + 9].Trim()) && addressee == null)
      {
        var message = string.Format("Не найден Адресат \"{2}\". Входящее письмо: \"{0} {1}\". ", regNumber, regDate.ToString(), this.Parameters[shift + 9].Trim());
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
        logger.Warn(message);
      }

      variableForParameters = this.Parameters[shift + 11].Trim();
      var deliveryMethod = BusinessLogic.GetEntityWithFilter<IMailDeliveryMethods>(m => m.Name == variableForParameters, exceptionList, logger);

      if (!string.IsNullOrEmpty(this.Parameters[shift + 11].Trim()) && deliveryMethod == null)
      {
        var message = string.Format("Не найден Способ доставки \"{2}\". Входящее письмо: \"{0} {1}\". ", regNumber, regDate.ToString(), this.Parameters[shift + 10].Trim());
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
        logger.Warn(message);
      }

			var documentRegisterIdStr = this.Parameters[shift + 12].Trim();
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

      var note = this.Parameters[shift + 10];

      var regState = this.Parameters[shift + 13].Trim();

      try
      {
        var isNewIncomingLetter = false;
        var incomingLetters = BusinessLogic.GetEntitiesWithFilter<IIncomingLetters>(x => x.RegistrationNumber == regNumber &&
			x.RegistrationDate.Value.ToString("d") == regDate.ToString("d") &&
			x.DocumentRegister.Id == documentRegisters.Id, exceptionList, logger, true);

        var incomingLetter = (IIncomingLetters)IOfficialDocuments.GetDocumentByRegistrationDate(incomingLetters, regDate, logger, exceptionList);
        if (incomingLetter == null)
        {
          incomingLetter = new IIncomingLetters();
          isNewIncomingLetter = true;
        }

        incomingLetter.Name = fileNameWithoutExtension;
        incomingLetter.Created = DateTimeOffset.UtcNow;
        incomingLetter.Correspondent = counterparty;
        incomingLetter.DocumentKind = documentKind;
        incomingLetter.Subject = subject;
        incomingLetter.Department = department;
        incomingLetter.BusinessUnit = department.BusinessUnit;

        if (department != null)
          incomingLetter.BusinessUnit = department.BusinessUnit;

				if (dated != DateTimeOffset.MinValue)
					incomingLetter.Dated = dated;
				else
					incomingLetter.Dated = null;

        incomingLetter.InNumber = inNumber;
        incomingLetter.Addressee = addressee;
        incomingLetter.DeliveryMethod = deliveryMethod;
        incomingLetter.Note = note;

        incomingLetter.DocumentRegister = documentRegisters;

				if (dated != DateTimeOffset.MinValue)
					incomingLetter.RegistrationDate = regDate.UtcDateTime;
				else
					incomingLetter.RegistrationDate = null;
        incomingLetter.RegistrationNumber = regNumber;
        if (!string.IsNullOrEmpty(incomingLetter.RegistrationNumber) && incomingLetter.DocumentRegister != null)
          incomingLetter.RegistrationState = BusinessLogic.GetRegistrationsState(regState);

        IIncomingLetters createdIncomingLetter;
        if (isNewIncomingLetter)
        {
          createdIncomingLetter = BusinessLogic.CreateEntity(incomingLetter, exceptionList, logger);
        }
        else
        {
          // Карточку не обновляем, там ошибка, если у документа есть версия.
          createdIncomingLetter = incomingLetter;//BusinessLogic.UpdateEntity(contract, exceptionList, logger);
        }

        if (createdIncomingLetter == null)
          return exceptionList;

        var update_body = ExtraParameters.ContainsKey("update_body") && ExtraParameters["update_body"] == "true";
        if (!string.IsNullOrWhiteSpace(filePath))
          exceptionList.AddRange(BusinessLogic.ImportBody(createdIncomingLetter, filePath, logger, update_body));
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
