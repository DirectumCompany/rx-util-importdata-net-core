using System;
using System.Collections.Generic;
using System.Globalization;
using NLog;
using ImportData.IntegrationServicesClient.Models;
using System.IO;
using DocumentFormat.OpenXml.Drawing.Charts;
using ImportData.Entities.Databooks;
using System.Linq;

namespace ImportData
{
  public class Contract : Entity
  {
    public int PropertiesCount = 21;
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

      variableForParameters = this.Parameters[shift + 4].Trim();
      var contractCategory = BusinessLogic.GetEntityWithFilter<IContractCategories>(c => c.Name == variableForParameters, exceptionList, logger);

      if (!string.IsNullOrEmpty(this.Parameters[shift + 4].ToString()))
      {
        if (contractCategory == null)
        {
          var message = string.Format("Не найдена категория договора \"{0}\".", this.Parameters[shift + 4]);
          exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
          logger.Error(message);

          return exceptionList;
        }
      }

      var subject = this.Parameters[shift + 5];

      variableForParameters = this.Parameters[shift + 6].Trim();
      var businessUnit = BusinessLogic.GetEntityWithFilter<IBusinessUnits>(b => b.Name == variableForParameters, exceptionList, logger);

      if (businessUnit == null)
      {
        var message = string.Format("Не найдена НОР \"{0}\".", this.Parameters[shift + 6]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      variableForParameters = this.Parameters[shift + 7].Trim();
      IDepartments department = null;
      if (businessUnit != null)
        department = BusinessLogic.GetEntityWithFilter<IDepartments>(d => d.Name == variableForParameters &&
        (d.BusinessUnit == null || d.BusinessUnit.Id == businessUnit.Id), exceptionList, logger, true);
      else
        department = BusinessLogic.GetEntityWithFilter<IDepartments>(d => d.Name == variableForParameters, exceptionList, logger);

      if (department == null)
      {
        var message = string.Format("Не найдено подразделение \"{0}\".", this.Parameters[shift + 7]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      var filePath = this.Parameters[shift + 8];
      var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

      DateTimeOffset validFrom = DateTimeOffset.MinValue;
      try
      {
        validFrom = ParseDate(this.Parameters[shift + 9], style, culture);
      }
      catch (Exception)
      {
        var message = string.Format("Не удалось обработать значение в поле \"Действует с\" \"{0}\".", this.Parameters[shift + 9]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      DateTimeOffset validTill = DateTimeOffset.MinValue;
      try
      {
        validTill = ParseDate(this.Parameters[shift + 10], style, culture);
      }
      catch (Exception)
      {
        var message = string.Format("Не удалось обработать значение в поле \"Действует по\" \"{0}\".", this.Parameters[shift + 10]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      var totalAmount = 0.0;

      if (!string.IsNullOrWhiteSpace(this.Parameters[shift + 11]) && !double.TryParse(this.Parameters[shift + 11].Trim(), style, culture, out totalAmount))
      {
        var message = string.Format("Не удалось обработать значение в поле \"Сумма\" \"{0}\".", this.Parameters[shift + 11]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      variableForParameters = this.Parameters[shift + 12].Trim();
      var currency = BusinessLogic.GetEntityWithFilter<ICurrencies>(c => c.Name == variableForParameters, exceptionList, logger);

      if (!string.IsNullOrEmpty(this.Parameters[shift + 12].Trim()) && currency == null)
      {
        var message = string.Format("Не найдено соответствующее наименование валюты \"{0}\".", this.Parameters[shift + 12]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      var lifeCycleState = BusinessLogic.GetPropertyLifeCycleState(this.Parameters[shift + 13]);

      if (!string.IsNullOrEmpty(this.Parameters[shift + 13].Trim()) && lifeCycleState == null)
      {
        var message = string.Format("Не найдено соответствующее значение состояния \"{0}\".", this.Parameters[shift + 13]);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
        logger.Error(message);

        return exceptionList;
      }

      variableForParameters = this.Parameters[shift + 14].Trim();
      var responsibleEmployee = BusinessLogic.GetEntityWithFilter<IEmployees>(e => e.Name == variableForParameters, exceptionList, logger);

      if (!string.IsNullOrEmpty(this.Parameters[shift + 14].Trim()) && responsibleEmployee == null)
      {
        var message = string.Format("Не найден Ответственный \"{3}\". Договор: \"{0} {1} {2}\". ", regNumber, regDate.ToString(), counterparty, this.Parameters[shift + 14].Trim());
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
        logger.Warn(message);
      }

      variableForParameters = this.Parameters[shift + 15].Trim();
      var ourSignatory = BusinessLogic.GetEntityWithFilter<IEmployees>(e => e.Name == variableForParameters, exceptionList, logger);

      if (!string.IsNullOrEmpty(this.Parameters[shift + 15].Trim()) && ourSignatory == null)
      {
        var message = string.Format("Не найден Подписывающий \"{3}\". Договор: \"{0} {1} {2}\". ", regNumber, regDate.ToString(), counterparty, this.Parameters[shift + 15].Trim());
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
        logger.Warn(message);
      }

      var note = this.Parameters[shift + 16];

      var documentRegisterIdStr = this.Parameters[shift + 17].Trim();
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
      var regState = this.Parameters[shift + 18].Trim();

      var caseFileStr = this.Parameters[shift + 19].Trim();
      var caseFile = BusinessLogic.GetEntityWithFilter<ICaseFiles>(x => x.Name == caseFileStr, exceptionList, logger);
      if (!string.IsNullOrEmpty(caseFileStr) && caseFile == null)
      {
        var message = string.Format("Не найдено Дело по наименованию \"{0}\"", caseFileStr);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
        logger.Error(message);
      }

      var placedToCaseFileDateStr = this.Parameters[shift + 20].Trim();
      DateTimeOffset placedToCaseFileDate = DateTimeOffset.MinValue;
      try
      {
        if (caseFile != null)
          placedToCaseFileDate = ParseDate(placedToCaseFileDateStr, style, culture);
      }
      catch (Exception)
      {
        var message = string.Format("Не удалось обработать значение поля \"Дата помещения\" \"{0}\".", placedToCaseFileDateStr);
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
        logger.Error(message);
      }

      try
      {
        var isNewContract = false;
        var contracts = BusinessLogic.GetEntitiesWithFilter<IContracts>(x => x.RegistrationNumber != null &&
            x.RegistrationNumber == regNumber &&
            x.RegistrationDate.Value.ToString("d") == regDate.ToString("d") &&
            x.Counterparty.Id == counterparty.Id &&
            x.DocumentRegister.Id == documentRegisters.Id, exceptionList, logger, true);

        var contract = (IContracts)IOfficialDocuments.GetDocumentByRegistrationDate(contracts, regDate, logger, exceptionList);
        if (contract == null)
        {
          contract = new IContracts();
          isNewContract = true;
        }

        // Обязательные поля.
        contract.Name = fileNameWithoutExtension;
        contract.Created = DateTimeOffset.UtcNow;
        contract.Counterparty = counterparty;
        contract.DocumentKind = documentKind;
        contract.DocumentGroup = contractCategory;
        contract.Subject = subject;
        contract.BusinessUnit = businessUnit;
        contract.Department = department;
        if (validFrom != DateTimeOffset.MinValue)
          contract.ValidFrom = validFrom;
        else
          contract.ValidFrom = null;
        if (validTill != DateTimeOffset.MinValue)
          contract.ValidTill = validTill;
        else
          contract.ValidTill = null;
        contract.TotalAmount = totalAmount;
        contract.Currency = currency;
        contract.ResponsibleEmployee = responsibleEmployee;
        contract.OurSignatory = ourSignatory;
        contract.Note = note;

        contract.DocumentRegister = documentRegisters;
        if (regDate != DateTimeOffset.MinValue)
          contract.RegistrationDate = regDate.UtcDateTime;
        else
          contract.RegistrationDate = null;
        contract.RegistrationNumber = regNumber;
        if (!string.IsNullOrEmpty(contract.RegistrationNumber) && contract.DocumentRegister != null)
          contract.RegistrationState = BusinessLogic.GetRegistrationsState(regState);

        contract.CaseFile = caseFile;
        if (placedToCaseFileDate != DateTimeOffset.MinValue)
          contract.PlacedToCaseFileDate = placedToCaseFileDate;
        else
          contract.PlacedToCaseFileDate = null;

        IContracts createdContract;
        if (isNewContract)
        {
          createdContract = BusinessLogic.CreateEntity(contract, exceptionList, logger);
          // Дополнительно обновляем свойство Состояние, так как после установки регистрационного номера Состояние сбрасывается в значение "В разработке"
          createdContract?.UpdateLifeCycleState(lifeCycleState);
        }
        else
        {
          // Карточку не обновляем, там ошибка, если у документа есть версия.
          createdContract = contract;//BusinessLogic.UpdateEntity(contract, exceptionList, logger);
        }

        if (createdContract == null)
          return exceptionList;

        var update_body = ExtraParameters.ContainsKey("update_body") && ExtraParameters["update_body"] == "true";
        if (!string.IsNullOrWhiteSpace(filePath))
          exceptionList.AddRange(BusinessLogic.ImportBody(createdContract, filePath, logger, update_body));
      }
      catch (Exception ex)
      {
        exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = ex.Message });
        logger.Error(ex, ex.Message);
        return exceptionList;
      }

      return exceptionList;
    }
  }
}
