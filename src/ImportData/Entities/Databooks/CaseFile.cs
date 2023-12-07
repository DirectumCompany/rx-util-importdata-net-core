using System;
using System.Collections.Generic;
using NLog;
using ImportData.IntegrationServicesClient.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ImportData.Entities.Databooks
{
    internal class CaseFile : Entity
    {
        public int PropertiesCount = 10;

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

            var index = this.Parameters[shift + 0].Trim();
            var title = this.Parameters[shift + 1].Trim();
            var retentionPeriodName = this.Parameters[shift + 2].Trim();
            var retentionPeriodValueXlsx = this.Parameters[shift + 3].Trim();
            var startDateXlsx = this.Parameters[shift + 4].Trim();
            var endDateXlsx = this.Parameters[shift + 5].Trim();
            var businessUnitName = this.Parameters[shift + 6].Trim();
            var departmentName = this.Parameters[shift + 7].Trim();
            var registrationGroupName = this.Parameters[shift + 8].Trim();
            var note = this.Parameters[shift + 9].Trim();

            var retentionPeriodValue = StrToNullableInt(retentionPeriodValueXlsx);
            DateTimeOffset? startDate = DateTimeOffset.MinValue;
            DateTimeOffset? endDate = DateTimeOffset.MinValue;

            var style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            var culture = CultureInfo.CreateSpecificCulture("en-GB");


            var message = @"Не заполнено поле ""{0}"".";
            if (string.IsNullOrWhiteSpace(index))
                return GetErrorResult(exceptionList, logger, message, "Индекс");
            
            if (string.IsNullOrWhiteSpace(title))
                return GetErrorResult(exceptionList, logger, message, "Заголовок");

            if (string.IsNullOrWhiteSpace(retentionPeriodName))
                return GetErrorResult(exceptionList, logger, message, "Наименование срока хранения");

            if (string.IsNullOrWhiteSpace(startDateXlsx))
                return GetErrorResult(exceptionList, logger, message, "Дата начала");

            message = @"Не удалось обработать значение в поле ""{0}"" ""{1}"".";

            if (string.IsNullOrWhiteSpace(retentionPeriodValueXlsx) && retentionPeriodValue == null)
                GetWarnResult(exceptionList, logger, message, "Срок хранения", retentionPeriodValueXlsx);

            try
            {
                startDate = ParseDate(startDateXlsx, style, culture);
            }
            catch
            {
                return GetErrorResult(exceptionList, logger, message, "Дата начала", startDateXlsx);
            }

            try
            {
                endDate = ParseDate(endDateXlsx, style, culture);
            }
            catch
            {
                return GetErrorResult(exceptionList, logger, message, "Дата окончания", endDateXlsx);
            }

            var businessUnit = string.IsNullOrWhiteSpace(businessUnitName) ? null : 
                BusinessLogic.GetEntityWithFilter<IBusinessUnits>(x => x.Name == businessUnitName, exceptionList, logger);
            var department = string.IsNullOrWhiteSpace(departmentName) ? null : 
                BusinessLogic.GetEntityWithFilter<IDepartments>(x => x.Name == departmentName, exceptionList, logger);
            var retentionPeriod = BusinessLogic.GetEntityWithFilter<IFileRetentionPeriods>(x => x.Name == retentionPeriodName &&
                (retentionPeriodValue == int.MinValue || x.RetentionPeriod == retentionPeriodValue), exceptionList, logger);
            var registrationGroup = string.IsNullOrWhiteSpace(registrationGroupName) ? null : 
                BusinessLogic.GetEntityWithFilter<IRegistrationGroups>(x => x.Name == registrationGroupName, exceptionList, logger);

            retentionPeriod ??= BusinessLogic.CreateEntity(new IFileRetentionPeriods() { Name = retentionPeriodName, RetentionPeriod = retentionPeriodValue, Status = "Active" }, 
                exceptionList, logger);

            try
            {
                ICaseFiles caseFile = null;
                var isNewCaseFile = false;

                if (ignoreDuplicates.ToLower() != Constants.ignoreDuplicates.ToLower())
                    caseFile = BusinessLogic.GetEntityWithFilter<ICaseFiles>(x => x.Index == index && x.Title == title, exceptionList, logger);

                if (caseFile == null)
                {
                    isNewCaseFile = true;
                    caseFile = new ICaseFiles();
                }

                caseFile.Index = index;
                caseFile.Title = title;
                caseFile.RetentionPeriod = retentionPeriod;
                caseFile.StartDate = startDate;
                caseFile.EndDate = endDate == DateTimeOffset.MinValue ? null : endDate;
                caseFile.BusinessUnit = businessUnit;
                caseFile.Department = department;
                caseFile.RegistrationGroup = registrationGroup;
                caseFile.Note = note;
                caseFile.Status = "Active";

                if (isNewCaseFile)
                    BusinessLogic.CreateEntity(caseFile, exceptionList, logger);
                else
                    BusinessLogic.UpdateEntity(caseFile, exceptionList, logger);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = ex.Message });

                return exceptionList;
            }

            return exceptionList;
        }

        public List<Structures.ExceptionsStruct> GetErrorResult(List<Structures.ExceptionsStruct> exceptionList, Logger logger, string message,  params string[] propertyName)
        {
            message = string.Format(message, propertyName);
            exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
            logger.Error(message);
            return exceptionList;
        }

        public List<Structures.ExceptionsStruct> GetWarnResult(List<Structures.ExceptionsStruct> exceptionList, Logger logger, string message, params string[] propertyName)
        {
            message = string.Format(message, propertyName);
            exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
            logger.Error(message);
            return exceptionList;
        }

        public int? StrToNullableInt(string str)
        {
            if (int.TryParse(str, out var i))
                return i;
            return null;
        }
    }
}
