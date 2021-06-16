using System;
using System.Collections.Generic;
using System.Globalization;
using NLog;
using System.Linq;
using ImportData.IntegrationServicesClient.Models;

namespace ImportData
{
    class DocumentRegister : Entity
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

            var name = this.Parameters[shift + 0].Trim();
            var variableForParameters = this.Parameters[shift + 0].Trim();

            if (string.IsNullOrEmpty(name))
            {
                var message = string.Format("Не заполнено поле \"Наименование\".");
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = message });
                logger.Error(message);

                return exceptionList;
            }

            var registerType = this.Parameters[shift + 1].Trim();
            var index = this.Parameters[shift + 2].Trim();

            variableForParameters = this.Parameters[shift + 3].Trim();
            var registrationGroup = BusinessLogic.GetEntityWithFilter<IRegistrationGroups>(c => c.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 3].Trim()) && registrationGroup == null)
            {
                var message = string.Format("Не найден Населенный пункт \"{1}\". Журнал регистрации: \"{0}\". ", name, this.Parameters[shift + 3].Trim());
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
                logger.Warn(message);
            }
            

            var documentFlow = this.Parameters[shift + 4].Trim();
            if (string.IsNullOrEmpty(documentFlow))
            {
                var message = string.Format("Не заполнено поле \"Документопоток\". Журнал регистрации: \"{0}\". ", name);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = message });
                logger.Error(message);
                return exceptionList;
            }

            var style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            var culture = CultureInfo.CreateSpecificCulture("en-GB");
            var numberOfDigitsInNumber = 0;
            if (!string.IsNullOrWhiteSpace(this.Parameters[shift + 5]) && !int.TryParse(this.Parameters[shift + 5].Trim(), style, culture, out numberOfDigitsInNumber))
            {
                var message = string.Format("Не удалось обработать кол-во цифр в номере. Журнал регистрации \"{0}\".", name);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);
                return exceptionList;
            }

            var numberingPeriod = this.Parameters[shift + 6].Trim();
            if (string.IsNullOrEmpty(numberingPeriod))
            {
                var message = string.Format("Не заполнено поле \"Документопоток\". Журнал регистрации: \"{0}\". ", name);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = message });
                logger.Error(message);
                return exceptionList;
            }

            var numberingSection = this.Parameters[shift + 7].Trim();
            if (string.IsNullOrEmpty(numberingSection))
            {
                var message = string.Format("Не заполнено поле \"Документопоток\". Журнал регистрации: \"{0}\". ", name);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = message });
                logger.Error(message);
                return exceptionList;
            }

            try
            {
                var documentRegister = new IDocumentRegisters();

                documentRegister.Name = name;
                documentRegister.RegisterType = registerType;
                documentRegister.Index = index;
                documentRegister.RegistrationGroup = registrationGroup;
                documentRegister.DocumentFlow = documentFlow;
                documentRegister.NumberOfDigitsInNumber = numberOfDigitsInNumber;
                documentRegister.NumberingPeriod = numberingPeriod;
                documentRegister.NumberingSection = numberingSection;
                documentRegister.Status = "Active";

                BusinessLogic.CreateEntity<IDocumentRegisters>(documentRegister, exceptionList, logger);
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
