using System;
using System.Collections.Generic;
using ImportData.IntegrationServicesClient.Models;
using NLog;

namespace ImportData
{
	class Country : Entity
	{
		public int PropertiesCount = 3;
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

			if (string.IsNullOrEmpty(name))
			{
				var message = string.Format("Не заполнено поле \"Наименование\".");
				exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = message });
				logger.Error(message);

				return exceptionList;
			}

			var status = this.Parameters[shift + 1].Trim();

			if (string.IsNullOrEmpty(status))
			{
				var message = string.Format("Не заполнено поле \"Статус\".");
				exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = message });
				logger.Error(message);

				return exceptionList;
			}

			var code = this.Parameters[shift + 2].Trim();

			if (string.IsNullOrEmpty(code))
			{
				var message = string.Format("Не заполнено поле \"Код\".");
				exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = message });
				logger.Error(message);

				return exceptionList;
			}

			try
			{
				ICountries country = null;
				var isNewCompany = false;

				if (ignoreDuplicates.ToLower() != Constants.ignoreDuplicates.ToLower())
					country = BusinessLogic.GetEntityWithFilter<ICountries>(x => x.Name == name || x.Code == code, exceptionList, logger);

				if (country is null)
				{
					isNewCompany = true;
					country = new ICountries();
				}

				country.Status = status;
				country.Name = name;
				country.Code = code;

				if (isNewCompany)
					BusinessLogic.CreateEntity(country, exceptionList, logger);
				else
					BusinessLogic.UpdateEntity(country, exceptionList, logger);
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
