using ImportData.IntegrationServicesClient.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ImportData
{
    class Addendum : Entity
    {
        public int PropertiesCount = 16;
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
            var variableForParameters = this.Parameters[shift + 0].Trim();
            var regNumber = variableForParameters;

            DateTimeOffset regDateLeadingDocument = DateTimeOffset.MinValue;
            DateTimeOffset regDate = DateTimeOffset.MinValue;
            var regNumberLeadingDocument = string.Empty;
            var documentKind = new IDocumentKinds();
            var subject = string.Empty;
            string lifeCycleState;
            var note = string.Empty;
            var leadingDocument = new IOfficialDocuments();
            var filePath = string.Empty;

            try
            {
                try
                {
                    regDate = ParseDate(this.Parameters[shift + 1], NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CreateSpecificCulture("en-GB"));
                }
                catch (Exception)
                {
                    var message = string.Format("Не удалось обработать дату регистрации \"{0}\".", this.Parameters[shift + 1]);
                    exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                    logger.Warn(message);

                    return exceptionList;
                }

                regNumberLeadingDocument = this.Parameters[shift + 2];
                try
                {
                    regDateLeadingDocument = ParseDate(this.Parameters[shift + 3], NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CreateSpecificCulture("en-GB"));
                }
                catch (Exception)
                {
                    var message = string.Format("Не удалось обработать дату ведущего документа \"{0}\".", this.Parameters[shift + 3]);
                    exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                    logger.Error(message);

                    return exceptionList;
                }

                variableForParameters = this.Parameters[shift + 5].Trim();
                documentKind = BusinessLogic.GetEntityWithFilter<IDocumentKinds>(d => d.Name == variableForParameters, exceptionList, logger);

                if (documentKind == null)
                {
                    var message = string.Format("Не найден вид документа \"{0}\".", this.Parameters[shift + 5]);
                    exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                    logger.Error(message);

                    return exceptionList;
                }

                subject = this.Parameters[shift + 6];

                variableForParameters = this.Parameters[shift + 7].Trim();
                var businessUnit = BusinessLogic.GetEntityWithFilter<IBusinessUnits>(c => c.Name == variableForParameters, exceptionList, logger);

                if (businessUnit == null)
                {
                    var message = string.Format("Не найдена НОР \"{0}\".", this.Parameters[shift + 7]);
                    exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                    logger.Error(message);

                    return exceptionList;
                }

                variableForParameters = this.Parameters[shift + 8].Trim();
                IDepartments department = null;
                if (businessUnit != null)
                    department = BusinessLogic.GetEntityWithFilter<IDepartments>(d => d.Name == variableForParameters &&
                    (d.BusinessUnit == null || d.BusinessUnit.Id == businessUnit.Id), exceptionList, logger, true);
                else
                    department = BusinessLogic.GetEntityWithFilter<IDepartments>(d => d.Name == variableForParameters, exceptionList, logger);

                if (department == null)
                {
                    var message = string.Format("Не найдено подразделение \"{0}\".", this.Parameters[shift + 8]);
                    exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                    logger.Error(message);

                    return exceptionList;
                }

                filePath = this.Parameters[shift + 9];
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

                lifeCycleState = BusinessLogic.GetPropertyLifeCycleState(this.Parameters[shift + 10]);

                if (!string.IsNullOrEmpty(this.Parameters[shift + 10].Trim()) && lifeCycleState == null)
                {
                    var message = string.Format("Не найдено соответствующее значение состояния \"{0}\".", this.Parameters[shift + 10]);
                    exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                    logger.Error(message);

                    return exceptionList;
                }

                note = this.Parameters[shift + 13].Trim();

                var leadDocResearchResult = IOfficialDocuments.GetLeadingDocument(logger, regNumberLeadingDocument, regDateLeadingDocument);
                leadingDocument = leadDocResearchResult.leadingDocument;
                if (!string.IsNullOrEmpty(leadDocResearchResult.errorMessage))
                {
                    var message = leadDocResearchResult.errorMessage;
                    exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                    logger.Error(message);

                    return exceptionList;
                }

                var documentRegisterIdStr = this.Parameters[shift + 14].Trim();
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

                var regState = this.Parameters[shift + 15].Trim();

                try
                {
                    var isNewAddendum = false;
                    var addendum = BusinessLogic.GetEntityWithFilter<IAddendums>(c => c.LeadingDocument.Id == leadingDocument.Id &&
                        c.DocumentKind.Id == documentKind.Id &&
                        c.Subject == subject &&
                        c.DocumentRegister.Id == documentRegisters.Id, exceptionList, logger);
                    if (addendum == null)
                    {
                        addendum = new IAddendums();
                        isNewAddendum = true;
                    }

                    // Обязательные поля.
                    addendum.Name = fileNameWithoutExtension;
                    addendum.Department = department;

                    addendum.Created = DateTimeOffset.UtcNow;
                    addendum.LeadingDocument = leadingDocument;
                    addendum.DocumentKind = documentKind;
                    addendum.Subject = subject;
                    addendum.LifeCycleState = lifeCycleState;
                    addendum.Note = note;

                    addendum.DocumentRegister = documentRegisters;
                    addendum.RegistrationNumber = regNumber;

                    if (regDate != DateTimeOffset.MinValue)
                        addendum.RegistrationDate = regDate.UtcDateTime;
                    else
                        addendum.RegistrationDate = null;

                    if (!string.IsNullOrEmpty(addendum.RegistrationNumber) && addendum.DocumentRegister != null)
                        addendum.RegistrationState = BusinessLogic.GetRegistrationsState(regState);

                    IAddendums createdAddendum;
                    if (isNewAddendum)
                    {
                        createdAddendum = BusinessLogic.CreateEntity(addendum, exceptionList, logger);
                        createdAddendum?.UpdateLifeCycleState(lifeCycleState);

                    }
                    else
                    {
                        // Карточку не обновляем, там ошибка, если у документа есть версия.
                        createdAddendum = addendum;//BusinessLogic.UpdateEntity(contract, exceptionList, logger);
                    }

                    if (createdAddendum == null)
                        return exceptionList;

                    var update_body = ExtraParameters.ContainsKey("update_body") && ExtraParameters["update_body"] == "true";
                    if (!string.IsNullOrWhiteSpace(filePath))
                        exceptionList.AddRange(BusinessLogic.ImportBody(createdAddendum, filePath, logger, update_body));
                }
                catch (Exception ex)
                {
                    exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = ex.Message });
                    logger.Error(ex.Message);
                }
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
