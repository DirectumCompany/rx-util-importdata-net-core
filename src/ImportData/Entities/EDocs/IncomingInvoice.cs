using System;
using System.Collections.Generic;
using System.Globalization;
using NLog;
using System.Linq;
using ImportData.IntegrationServicesClient.Models;

namespace ImportData
{
    class IncomingInvoice : Entity
    {
        public int PropertiesCount = 8;
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
            var numberInvoice = this.Parameters[shift + 0];
            DateTime? dateInvoice = DateTime.MinValue;
            var style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            var culture = CultureInfo.CreateSpecificCulture("en-GB");
            var dateInvoiceDouble = 0.0;
            if (!string.IsNullOrWhiteSpace(this.Parameters[shift + 1]) && !double.TryParse(this.Parameters[shift + 1].Trim(), style, culture, out dateInvoiceDouble))
            {
                var message = string.Format("Не удалось обработать дату счета \"{0}\".", this.Parameters[shift + 1]);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);
                return exceptionList;
            }
            else
            {
                if (!string.IsNullOrEmpty(this.Parameters[shift + 1].ToString()))
                    dateInvoice = DateTime.FromOADate(dateInvoiceDouble);
            }

            var variableForParameters = this.Parameters[shift + 2].Trim();
            var documentKind = BusinessLogic.GetEntityWithFilter<IDocumentKinds>(c => c.Name == variableForParameters, exceptionList, logger);
            if (documentKind == null)
            {
                var message = string.Format("Не найден вид документа \"{0}\".", this.Parameters[shift + 2]);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);
                return exceptionList;
            }

            variableForParameters = this.Parameters[shift + 3].Trim();
            var counterparty = BusinessLogic.GetEntityWithFilter<ICounterparties>(c => c.Name == variableForParameters, exceptionList, logger);
            if (counterparty == null)
            {
                var message = string.Format("Не найден контрагент \"{0}\".", this.Parameters[shift + 3]);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);
                return exceptionList;
            }

            var totalAmount = 0.0;
            if (!string.IsNullOrWhiteSpace(this.Parameters[shift + 4]) && !double.TryParse(this.Parameters[shift + 4].Trim(), style, culture, out totalAmount))
            {
                var message = string.Format("Не удалось обработать значение в поле \"Сумма\" \"{0}\".", this.Parameters[shift + 4]);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);
                return exceptionList;
            }

            variableForParameters = this.Parameters[shift + 5].Trim();
            var currency = BusinessLogic.GetEntityWithFilter<ICurrency>(c => c.Name == variableForParameters, exceptionList, logger);
            if (currency == null)
            {
                var message = string.Format("Не найдена валюта \"{0}\".", this.Parameters[shift + 5]);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);
                return exceptionList;
            }

            variableForParameters = this.Parameters[shift + 6].Trim();
            var businessUnit = BusinessLogic.GetEntityWithFilter<IBusinessUnits>(c => c.Name == variableForParameters, exceptionList, logger);
            if (businessUnit == null)
            {
                var message = string.Format("Не найдена Наша.орг \"{0}\".", this.Parameters[shift + 6]);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);
                return exceptionList;
            }

            variableForParameters = this.Parameters[shift + 7].Trim();
            var department = BusinessLogic.GetEntityWithFilter<IDepartments>(c => c.Name == variableForParameters, exceptionList, logger);
            if (department == null)
            {
                var message = string.Format("Не найдено подразделение \"{0}\".", this.Parameters[shift + 7]);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);
                return exceptionList;
            }

            try
            {
                var incomingInvoice = new IIncomingInvoices();
                incomingInvoice.Name = string.Format("\"{0}\" от \"{1}\"", documentKind.Name, counterparty.Name);
                incomingInvoice.Number = numberInvoice;
                incomingInvoice.Date = dateInvoice;
                incomingInvoice.DocumentKind = documentKind;
                incomingInvoice.Counterparty = counterparty;
                incomingInvoice.TotalAmount = totalAmount;
                incomingInvoice.Currency = currency;
                incomingInvoice.BusinessUnit = businessUnit;
                incomingInvoice.Department = department;

                var createdIncomingInvoice = BusinessLogic.CreateEntity<IIncomingInvoices>(incomingInvoice, exceptionList, logger);

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
