using ImportData.IntegrationServicesClient.Models;
using NLog;
using System;
using System.Collections.Generic;

namespace ImportData
{
    class Company : Entity
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
        public override IEnumerable<Structures.ExceptionsStruct> SaveToRX(Logger logger, string ignoreDuplicates, int shift = 0)
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

            var legalName = this.Parameters[shift + 1].Trim();

            variableForParameters = this.Parameters[shift + 2].Trim();
            ICompanies headCompany = null;
            if (!string.IsNullOrEmpty(variableForParameters))
            {
                var counterparty = BusinessLogic.GetEntityWithFilter<ICounterparties>(c => c.Name == variableForParameters, exceptionList, logger);
                headCompany = counterparty == null ?
                    BusinessLogic.CreateEntity<ICompanies>(new ICompanies() { Name = variableForParameters, Status = "Active" }, exceptionList, logger) :
                    ICompanies.CastCounterpartyToCompany(counterparty);
            }

            var nonresident = this.Parameters[shift + 3].ToLower() == "да" ? true : false;
            var tin = this.Parameters[shift + 4].Trim(); // ИНН
            var trrc = this.Parameters[shift + 5].Trim(); // КПП
            var psrn = this.Parameters[shift + 6].Trim(); // ОГРН
            var nceo = this.Parameters[shift + 7].Trim(); // ОКПО
            var ncea = this.Parameters[shift + 8].Trim(); // ОКВЭД

            variableForParameters = this.Parameters[shift + 9].Trim();
            var city = BusinessLogic.GetEntityWithFilter<ICities>(c => c.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 9].Trim()) && city == null)
            {
                var message = string.Format("Не найден Населенный пункт \"{1}\". Наименование организации: \"{0}\". ", name, this.Parameters[shift + 9].Trim());
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
                logger.Warn(message);
            }

            variableForParameters = this.Parameters[shift + 10].Trim();
            var region = BusinessLogic.GetEntityWithFilter<IRegions>(r => r.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 10].Trim()) && region == null)
            {
                var message = string.Format("Не найден Регион \"{1}\". Наименование организации: \"{0}\". ", name, this.Parameters[shift + 10].Trim());
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
                logger.Warn(message);
            }

            var legalAdress = this.Parameters[shift + 11].Trim();
            var postalAdress = this.Parameters[shift + 12].Trim();
            var phones = this.Parameters[shift + 13].Trim();
            var email = this.Parameters[shift + 14].Trim();
            var homepage = this.Parameters[shift + 15].Trim();
            var note = this.Parameters[shift + 16].Trim();
            var account = this.Parameters[shift + 17].Trim();

            variableForParameters = this.Parameters[shift + 18].Trim();
            var bank = BusinessLogic.GetEntityWithFilter<IBanks>(b => b.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 18]) && bank == null)
            {
                var message = string.Format("Не найден Банк \"{1}\". Наименование организации: \"{0}\". ", name, this.Parameters[shift + 18].Trim());
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
                logger.Warn(message);
            }

            variableForParameters = this.Parameters[shift + 19].Trim();
            var responsible = BusinessLogic.GetEntityWithFilter<IEmployees>(e => e.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 19]) && responsible == null)
            {
                var message = string.Format("Не найден Ответственный \"{1}\". Наименование организации: \"{0}\". ", name, this.Parameters[shift + 19].Trim());
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
                logger.Warn(message);
            }

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

            try
            {
                ICompanies company = null;
                var isNewCompany = false;

                if (ignoreDuplicates.ToLower() != Constants.ignoreDuplicates.ToLower())
                    company = BusinessLogic.GetEntityWithFilter<ICompanies>(x => x.Name == name ||
                        (tin != null && tin != string.Empty && x.TIN == tin &&
                         trrc != null && trrc != string.Empty && x.TRRC == trrc) ||
                        (psrn != null && psrn != string.Empty && x.PSRN == psrn), exceptionList, logger);

                if (company is null)
                {
                    isNewCompany = true;
                    company = new ICompanies();
                }

                company.Name = name;
                company.LegalName = legalName;
                company.HeadCompany = headCompany;
                company.Nonresident = nonresident;
                //company.Code = code;
                company.TIN = tin;
                company.TRRC = trrc;
                company.PSRN = psrn;
                company.NCEO = nceo;
                company.NCEA = ncea;
                company.City = city;
                company.Region = region;
                company.LegalAddress = legalAdress;
                company.PostalAddress = postalAdress;
                company.Phones = phones;
                company.Email = email;
                company.Homepage = homepage;
                company.Note = note;
                company.Account = account;
                company.Bank = bank;
                company.Status = "Active";
                company.Responsible = responsible;

                if (isNewCompany)
                    BusinessLogic.CreateEntity(company, exceptionList, logger);
                else
                    BusinessLogic.UpdateEntity(company, exceptionList, logger);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = ex.Message });
                logger.Error(ex, ex.Message);
                return exceptionList;
            }

            return exceptionList;
        }
    }
}
