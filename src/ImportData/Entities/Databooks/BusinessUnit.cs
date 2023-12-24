using System;
using System.Collections.Generic;
using NLog;
using ImportData.IntegrationServicesClient.Models;

namespace ImportData
{
    class BusinessUnit : Entity
    {
        public int PropertiesCount = 20;
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

            var name = this.Parameters[shift + 0].Trim();

            if (string.IsNullOrEmpty(name))
            {
                var message = string.Format("Не заполнено поле \"Наименование\".");
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = message });
                logger.Error(message);

                return exceptionList;
            }

            var legalName = this.Parameters[shift + 1].Trim();
            variableForParameters = this.Parameters[shift + 2].Trim();
            var headCompany = BusinessLogic.GetEntityWithFilter<IBusinessUnits>(b => b.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 2].Trim()) && headCompany == null)
            {
                headCompany = BusinessLogic.CreateEntity<IBusinessUnits>(new IBusinessUnits() { Name = variableForParameters, Status = "Active" }, exceptionList, logger);
            }

            variableForParameters = this.Parameters[shift + 3].Trim();
            var ceo = BusinessLogic.GetEntityWithFilter<IEmployees>(e => e.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 3].Trim()) && ceo == null)
            {
                var message = string.Format("Не найден Руководитель \"{1}\". Наименование НОР: \"{0}\". ", name, this.Parameters[shift + 3].Trim());
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
                logger.Warn(message);
            }

            var nonresident = this.Parameters[shift + 4].ToLower() == "да" ? true : false;

            var tin = this.Parameters[shift + 5].Trim();
            var trrc = this.Parameters[shift + 6].Trim();
            var psrn = this.Parameters[shift + 7].Trim();
            var nceo = this.Parameters[shift + 8].Trim();
            var ncea = this.Parameters[shift + 9].Trim();

            // Проверка ИНН.
            var resultTIN = BusinessLogic.CheckTin(tin, true, nonresident);

            if (!string.IsNullOrEmpty(resultTIN))
            {
                var message = string.Format("Компания не может быть импортирована. Некорректный ИНН. Наименование: \"{0}\", ИНН: {1}. {2}", name, tin, resultTIN);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);

                return exceptionList;
            }

            // Проверка КПП.
            var resultTRRC = BusinessLogic.CheckTrrcLength(trrc, nonresident);

            if (!string.IsNullOrEmpty(resultTRRC))
            {
                var message = string.Format("Компания не может быть импортирована. Некорректный КПП. Наименование: \"{0}\", КПП: {1}. {2}", name, trrc, resultTRRC);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);

                return exceptionList;
            }

            // Проверка ОГРН.
            var resultPSRN = BusinessLogic.CheckPsrnLength(psrn, nonresident);

            if (!string.IsNullOrEmpty(resultPSRN))
            {
                var message = string.Format("Компания не может быть импортирована. Некорректный ОГРН. Наименование: \"{0}\", ОГРН: {1}. {2}", name, psrn, resultPSRN);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);

                return exceptionList;
            }

            // Проверка ОКПО.
            var resultNCEO = BusinessLogic.CheckNceoLength(nceo, nonresident);
            if (!string.IsNullOrEmpty(resultNCEO))
            {
                var message = string.Format("Компания не может быть импортирована. Некорректный ОКПО. Наименование: \"{0}\", ОКПО: {1}. {2}", name, nceo, resultNCEO);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);

                return exceptionList;
            }

            variableForParameters = this.Parameters[shift + 10].Trim();
            var city = BusinessLogic.GetEntityWithFilter<ICities>(c => c.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 10].Trim()) && city == null)
            {
                var message = string.Format("Не найден Населенный пункт \"{1}\". Наименование НОР: \"{0}\". ", name, this.Parameters[shift + 9].Trim());
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
                logger.Warn(message);
            }

            variableForParameters = this.Parameters[shift + 11].Trim();
            var region = BusinessLogic.GetEntityWithFilter<IRegions>(r => r.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 11].Trim()) && region == null)
            {
                var message = string.Format("Не найден Регион \"{1}\". Наименование НОР: \"{0}\". ", name, this.Parameters[shift + 10].Trim());
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
                logger.Warn(message);
            }

            var legalAdress = this.Parameters[shift + 12].Trim();
            var postalAdress = this.Parameters[shift + 13].Trim();
            var phones = this.Parameters[shift + 14].Trim();
            var email = this.Parameters[shift + 15].Trim();
            var homepage = this.Parameters[shift + 16].Trim();
            var note = this.Parameters[shift + 17].Trim();
            var account = this.Parameters[shift + 18].Trim();

            variableForParameters = this.Parameters[shift + 19].Trim();
            var bank = BusinessLogic.GetEntityWithFilter<IBanks>(b => b.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 19]) && bank == null)
            {
                var message = string.Format("Не найден Банк \"{1}\". Наименование НОР: \"{0}\". ", name, this.Parameters[shift + 19].Trim());
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
                logger.Warn(message);
            }

            try
            {
                IBusinessUnits businessUnit = null;
                var isNewBU = false;

                if (ignoreDuplicates.ToLower() != Constants.ignoreDuplicates.ToLower())
                {
                    if (!string.IsNullOrEmpty(tin) && !string.IsNullOrEmpty(trrc) && !string.IsNullOrEmpty(psrn))
                        businessUnit = BusinessLogic.GetEntityWithFilter<IBusinessUnits>(x => x.Name == name && x.TIN == tin && x.TRRC == trrc && x.PSRN == psrn, exceptionList, logger);
                    else
                        businessUnit = BusinessLogic.GetEntityWithFilter<IBusinessUnits>(x => x.Name == name, exceptionList, logger);
                }

                if (businessUnit is null)
                {
                    isNewBU = true;
                    businessUnit = new IBusinessUnits();
                }

                businessUnit.Name = name;
                businessUnit.LegalName = legalName;
                businessUnit.HeadCompany = headCompany;
                businessUnit.Nonresident = nonresident;
                businessUnit.CEO = ceo;
                businessUnit.TIN = tin;
                businessUnit.TRRC = trrc;
                businessUnit.PSRN = psrn;
                businessUnit.NCEO = nceo;
                businessUnit.NCEA = ncea;
                businessUnit.City = city;
                businessUnit.Region = region;
                businessUnit.LegalAddress = legalAdress;
                businessUnit.PostalAddress = postalAdress;
                businessUnit.Phones = phones;
                businessUnit.Email = email;
                businessUnit.Homepage = homepage;
                businessUnit.Note = note;
                businessUnit.Account = account;
                businessUnit.Bank = bank;
                businessUnit.Status = "Active";

                if (isNewBU)
                    BusinessLogic.CreateEntity(businessUnit, exceptionList, logger);
                else
                    BusinessLogic.UpdateEntity(businessUnit, exceptionList, logger);
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
