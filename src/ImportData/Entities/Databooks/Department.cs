using System;
using System.Collections.Generic;
using ImportData.IntegrationServicesClient.Models;
using NLog;

namespace ImportData
{
    class Department : Entity
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
        public override IEnumerable<Structures.ExceptionsStruct> SaveToRX(Logger logger, string ignoreDuplicates, int shift = 0)
        {
            var exceptionList = new List<Structures.ExceptionsStruct>();
            var variableForParameters = this.Parameters[shift + 0].Trim();

            var nameDepartment = this.Parameters[shift + 0].Trim();

            if (string.IsNullOrEmpty(nameDepartment))
            {
                var message = string.Format("Не заполнено поле \"Наименование\".");
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                logger.Error(message);

                return exceptionList;
            }

            var shortName = this.Parameters[shift + 1].Trim();

            var code = this.Parameters[shift + 2].Trim();

            variableForParameters = this.Parameters[shift + 3].Trim();
            var businessUnit = BusinessLogic.GetEntityWithFilter<IBusinessUnits>(x => x.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 3].Trim()) && businessUnit == null)
            {
                var message = string.Format("Не найдена НОР \"{1}\". Наименование подразделения: \"{0}\". ", nameDepartment, this.Parameters[shift + 3].Trim());
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
                logger.Warn(message);
            }


            variableForParameters = this.Parameters[shift + 4].Trim();
            var headOffice = BusinessLogic.GetEntityWithFilter<IDepartments>(x => x.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 4].Trim()) && headOffice == null)
            {
                headOffice = BusinessLogic.CreateEntity<IDepartments>(new IDepartments() { Name = variableForParameters, Status = "Active" }, exceptionList, logger);
            }

            variableForParameters = this.Parameters[shift + 5].Trim();
            var manager = BusinessLogic.GetEntityWithFilter<IEmployees>(x => x.Name == variableForParameters, exceptionList, logger);

            if (!string.IsNullOrEmpty(this.Parameters[shift + 5].Trim()) && manager == null)
            {
                var message = string.Format("Не найден Руководитель \"{1}\". Наименование подразделения: \"{0}\". ", nameDepartment, this.Parameters[shift + 5].Trim());
                exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = message });
                logger.Warn(message);
            }

            var phone = this.Parameters[shift + 6].Trim();
            var note = this.Parameters[shift + 7].Trim();

            try
            {
                // Проверка кода подразделения.
                var resultCodeDepartment = BusinessLogic.CheckCodeDepartmentLength(code);

                if (!string.IsNullOrEmpty(resultCodeDepartment))
                {
                    var message = string.Format("Подразделение не может быть импортировано. Некорректный код подразделения. Наименование: \"{0}\", Код подразделения: {1}. {2}", nameDepartment, code, resultCodeDepartment);
                    exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
                    logger.Error(message);

                    return exceptionList;
                }

                IDepartments department = null;
                var isNewDepartment = false;

                if (ignoreDuplicates.ToLower() != Constants.ignoreDuplicates.ToLower())
                    department = BusinessLogic.GetEntityWithFilter<IDepartments>(x => x.Name == nameDepartment, exceptionList, logger);

                if (department is null)
                {
                    isNewDepartment = true;
                    department = new IDepartments();
                }

                department.Name = nameDepartment;
                department.ShortName = shortName;
                department.BusinessUnit = businessUnit;
                department.HeadOffice = headOffice;
                department.Manager = manager;
                department.Code = code;
                department.Phone = phone;
                department.Note = note;
                department.Status = "Active";

                if (isNewDepartment)
                    BusinessLogic.CreateEntity(department, exceptionList, logger);
                else
                    BusinessLogic.UpdateEntity(department, exceptionList, logger);
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
