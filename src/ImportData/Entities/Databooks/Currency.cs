using System;
using System.Collections.Generic;
using System.Linq;
using ImportData.IntegrationServicesClient.Models;
using NLog;

namespace ImportData
{
	class Currency : Entity
	{
		public int PropertiesCount = 6;
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

			var shortName = this.Parameters[shift + 1].Trim();

			if (string.IsNullOrEmpty(shortName))
			{
				var message = string.Format("Не заполнено поле \"Сокр. наименование\".");
				exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = message });
				logger.Error(message);

				return exceptionList;
			}

			var fractionName = this.Parameters[shift + 2].Trim();

			if (string.IsNullOrEmpty(fractionName))
			{
				var message = string.Format("Не заполнено поле \"Дробная часть\".");
				exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = message });
				logger.Error(message);

				return exceptionList;
			}

			var alphaCode = this.Parameters[shift + 3].Trim();

			if (string.IsNullOrEmpty(fractionName))
			{
				var message = string.Format("Не заполнено поле \"Буквенный код\".");
				exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = message });
				logger.Error(message);

				return exceptionList;
			}

			var numericCode = this.Parameters[shift + 4].Trim();

			if (string.IsNullOrEmpty(fractionName))
			{
				var message = string.Format("Не заполнено поле \"Цифровой код\".");
				exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = message });
				logger.Error(message);

				return exceptionList;
			}

			var status = this.Parameters[shift + 5].Trim();

			if (string.IsNullOrEmpty(status))
			{
				var message = string.Format("Не заполнено поле \"Статус\".");
				exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = "Error", Message = message });
				logger.Error(message);

				return exceptionList;
			}

			try
			{
				ICurrencies currency = null;
				var isNewCompany = false;

				if (ignoreDuplicates.ToLower() != Constants.ignoreDuplicates.ToLower())
					currency = BusinessLogic.GetEntityWithFilter<ICurrencies>(x => x.Name == name, exceptionList, logger);

				if (currency is null)
				{
					isNewCompany = true;
					currency = new ICurrencies();
				}

				currency.Name = name;
				currency.AlphaCode = alphaCode;
				currency.ShortName = shortName;
				currency.FractionName = fractionName;
				currency.NumericCode = numericCode;
				currency.Status = status;

				if (isNewCompany)
					BusinessLogic.CreateEntity(currency, exceptionList, logger);
				else
					BusinessLogic.UpdateEntity(currency, exceptionList, logger);
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
