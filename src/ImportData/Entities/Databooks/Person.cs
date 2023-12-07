using System;
using System.Collections.Generic;
using System.Globalization;
using NLog;
using ImportData.IntegrationServicesClient.Models;
using System.Linq.Expressions;

namespace ImportData
{
    class Person : Entity
    {
        public int PropertiesCount = 17;
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

            var lastName = this.Parameters[shift + 0].Trim();

            if (string.IsNullOrEmpty(lastName))
            {
                var message = string.Format("Не заполнено поле \"Фамилия\".");
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = message });
                logger.Error(message);

                return exceptionList;
            }

            var firstName = this.Parameters[shift + 1].Trim();

            if (string.IsNullOrEmpty(firstName))
            {
                var message = string.Format("Не заполнено поле \"Имя\".");
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = message });
                logger.Error(message);

                return exceptionList;
            }

            var middleName = this.Parameters[shift + 2].Trim();

            var sex = BusinessLogic.GetPropertySex(this.Parameters[shift + 3].Trim());
            DateTimeOffset? dateOfBirth = DateTimeOffset.MinValue;
            var culture = CultureInfo.CreateSpecificCulture("en-GB");
            try
            {
                dateOfBirth = ParseDate(this.Parameters[shift + 4], NumberStyles.Number | NumberStyles.AllowCurrencySymbol, culture);
            }
            catch (Exception)
            {
                var message = string.Format("Не удалось обработать значение в поле \"Дата рождения\" \"{0}\".", this.Parameters[shift + 4]);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
                logger.Warn(message);

            }

            var tin = this.Parameters[shift + 5].Trim();
            var inila = this.Parameters[shift + 6].Trim();

            variableForParameters = this.Parameters[shift + 7].Trim();
            var city = BusinessLogic.GetEntityWithFilter<ICities>(c => c.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 7].Trim()) && city == null)
            {
                var message = string.Format("Не найден Населенный пункт \"{3}\". Персона: \"{0} {1} {2}\". ", lastName, firstName, middleName, this.Parameters[shift + 7].Trim());
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
                logger.Warn(message);
            }

            variableForParameters = this.Parameters[shift + 8].Trim();
            var region = BusinessLogic.GetEntityWithFilter<IRegions>(r => r.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 8].Trim()) && region == null)
            {
                var message = string.Format("Не найден Регион \"{3}\". Персона: \"{0} {1} {2}\". ", lastName, firstName, middleName, this.Parameters[shift + 8].Trim());
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
                logger.Warn(message);
            }

            var legalAdress = this.Parameters[shift + 9].Trim();
            var postalAdress = this.Parameters[shift + 10].Trim();
            var phones = this.Parameters[shift + 11].Trim();
            var email = this.Parameters[shift + 12].Trim();
            var homepage = this.Parameters[shift + 13].Trim();

            variableForParameters = this.Parameters[shift + 14].Trim();
            var bank = BusinessLogic.GetEntityWithFilter<IBanks>(b => b.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 14].Trim()) && bank == null)
            {
                var message = string.Format("Не найден Банк \"{3}\". Персона: \"{0} {1} {2}\". ", lastName, firstName, middleName, this.Parameters[shift + 14].Trim());
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
                logger.Warn(message);
            }

            var account = this.Parameters[shift + 15].Trim();
            var note = this.Parameters[shift + 16].Trim();

            try
            {
                IPersons person = null;
                var isNewPerson = false;

                if (ignoreDuplicates.ToLower() != Constants.ignoreDuplicates.ToLower())
                {
                    person = BusinessLogic.GetEntityWithFilter<IPersons>(x => x.LastName == lastName && x.FirstName == firstName && x.MiddleName == middleName, exceptionList, logger);
                }

                if (person is null)
                {
                    isNewPerson = true;
                    person = new IPersons();
                }

                person.Name = string.Format("{0} {1} {2}", lastName, firstName, middleName);
                person.LastName = lastName;
                person.FirstName = firstName;
                person.MiddleName = middleName;
                person.Sex = sex;
                person.DateOfBirth = dateOfBirth != DateTimeOffset.MinValue ? dateOfBirth : null;
                person.TIN = tin;
                person.INILA = inila;
                person.City = city;
                person.Region = region;
                person.LegalAddress = legalAdress;
                person.PostalAddress = postalAdress;
                person.Phones = phones;
                person.Email = email;
                person.Homepage = homepage;
                person.Bank = bank;
                person.Account = account;
                person.Note = note;
                person.Status = "Active";

                if (isNewPerson)
                    BusinessLogic.CreateEntity(person, exceptionList, logger);
                else
                    BusinessLogic.UpdateEntity(person, exceptionList, logger);
            }
            catch (Exception ex)
            {
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = ex.Message });

                return exceptionList;
            }

            return exceptionList;
        }
    }
}
