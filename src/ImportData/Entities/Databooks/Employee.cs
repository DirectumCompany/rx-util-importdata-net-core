using System;
using System.Collections.Generic;
using ImportData.IntegrationServicesClient.Models;
using NLog;

namespace ImportData
{
    class Employee : Person
    {
        public int PropertiesCount = 19;
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

            exceptionList.AddRange(base.SaveToRX(logger, supplementEntity, ignoreDuplicates, 2));

            var lastName = this.Parameters[shift + 2].Trim();

            if (string.IsNullOrEmpty(lastName))
            {
                var message = string.Format("Не заполнено поле \"Фамилия\".");
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);

                return exceptionList;
            }

            var firstName = this.Parameters[shift + 3].Trim();

            if (string.IsNullOrEmpty(firstName))
            {
                var message = string.Format("Не заполнено поле \"Имя\".");
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);

                return exceptionList;
            }

            var middleName = this.Parameters[shift + 4].Trim();

            var person = BusinessLogic.CreateEntity<IPersons>(new IPersons() { FirstName = firstName, MiddleName = middleName, LastName = lastName, Name = string.Format("{0} {1} {2}", lastName, firstName, middleName), Status = "Active" }, exceptionList, logger);

            if (person == null)
            {
                var message = string.Format("Не удалось создать персону \"{0} {1} {2}\".", lastName, firstName, middleName);
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);

                return exceptionList;
            }


            variableForParameters = this.Parameters[shift + 0].Trim();
            var department = BusinessLogic.GetEntityWithFilter<IDepartments>(x => x.Name == variableForParameters, exceptionList, logger);

            if (department == null && !string.IsNullOrEmpty(this.Parameters[shift + 0].Trim()))
            {
                department = BusinessLogic.CreateEntity<IDepartments>(new IDepartments() { Name = variableForParameters, Status = "Active" }, exceptionList, logger);
            }

            variableForParameters = this.Parameters[shift + 1].Trim();
            var jobTitle = BusinessLogic.GetEntityWithFilter<IJobTitles>(x => x.Name == variableForParameters, exceptionList, logger);

            if (jobTitle == null && !string.IsNullOrEmpty(this.Parameters[shift + 1].Trim()))
            {
                jobTitle = BusinessLogic.CreateEntity<IJobTitles>(new IJobTitles() { Name = variableForParameters, Status = "Active" }, exceptionList, logger);
            }

            var email = this.Parameters[shift + 14].Trim();
            var phone = this.Parameters[shift + 13].Trim();
            var note = this.Parameters[shift + 18].Trim();

            try
            {
                if (ignoreDuplicates.ToLower() != Constants.ignoreDuplicates.ToLower())
                {
                    var employees = BusinessLogic.GetEntityWithFilter<IEmployees>(x => x.Name == person.Name, exceptionList, logger);

                    // Обновление сущности при условии, что найдено одно совпадение.
                    if (employees != null)
                    {
                        employees.Name = person.Name;
                        employees.Person = person;
                        employees.Department = department;
                        employees.JobTitle = jobTitle;
                        employees.Email = email;
                        employees.Phone = phone;
                        employees.Note = note;
                        employees.NeedNotifyExpiredAssignments = false;
                        employees.NeedNotifyNewAssignments = false;
                        employees.Status = "Active";

                        var updatedEntity = BusinessLogic.UpdateEntity<IEmployees>(employees, exceptionList, logger);

                        return exceptionList;
                    }
                }

                var employee = new IEmployees();

                employee.Name = person.Name;
                employee.Person = person;
                employee.Department = department;
                employee.JobTitle = jobTitle;
                employee.Email = email;
                employee.Phone = phone;
                employee.Note = note;
                employee.NeedNotifyExpiredAssignments = false;
                employee.NeedNotifyNewAssignments = false;
                employee.Status = "Active";

                BusinessLogic.CreateEntity<IEmployees>(employee, exceptionList, logger);
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
