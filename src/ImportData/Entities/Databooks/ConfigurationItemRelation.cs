using ImportData.IntegrationServicesClient;
using ImportData.IntegrationServicesClient.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace ImportData.Entities.Databooks
{
	public class ConfigurationItemRelation : Entity
	{
		public override int PropertiesCount { get { return 5; } }
		protected override Type EntityType { get { return typeof(ICMDBConfigurationItemRelations); } }

		/// <summary>
		/// Обработать событие "После сохранения" сущности.
		/// </summary>
		/// <param name="logger">Логировщик.</param>
		/// <param name="ignoreDuplicates">Признак необходимости игнорировать дубликаты.</param>
		/// <param name="isBatch">Признак необходимости пакетной загрузки.</param>
		/// <returns>Список ошибок и предупреждений.</returns>
		public override IEnumerable<Structures.ExceptionsStruct> SaveToRX(Logger logger, string ignoreDuplicates, bool isBatch = false)
		{
			//// Вместо логики занесения в RX необходимо вызвать функции интеграции для занесения связи на прямую в БД,
			//// так-как у связей объектной модели нету и стандартным функционалом утилиты данное требование не закрыть.

			var exceptionList = new List<Structures.ExceptionsStruct>();
			ResultValues = new Dictionary<string, object>();

			var properties = EntityType.GetProperties();
			foreach (var property in properties)
			{
				var options = BusinessLogic.GetPropertyOptions(property);
				if (options == null)
					continue;

				object variableForParameters = null;
				// Обработка свойств модели, которые заполняются/создаются из нескольких столбцов шаблона.
				if (options.Characters == AdditionalCharacters.CreateFromOtherProperties)
				{
					var propertiesForSearch = GetPropertiesForSearch(property.PropertyType, exceptionList, logger);

					if (propertiesForSearch == null)
						return exceptionList;

					// При обработке сущности сначала выполняется поиск сущности для тех сущностей
					// создание дублей которых избыточно и не требуется. Если сущность найдена - возвращается сущность, иначе создается новая.
					variableForParameters = MethodCall(property.PropertyType, Constants.EntityActions.FindEntity, propertiesForSearch, this, false, exceptionList, logger);

					if (variableForParameters == null)
						variableForParameters = MethodCall(property.PropertyType, Constants.EntityActions.CreateEntity, propertiesForSearch, this, exceptionList, isBatch, logger);
				}
				else
				{
					if (!NamingParameters.ContainsKey(options.ExcelName))
						continue;

					variableForParameters = NamingParameters[options.ExcelName].Trim();
					if (options.IsRequired())
					{
						if (CheckPropertyNull(options, variableForParameters, Constants.Resources.EmptyColumn, exceptionList, logger) == Constants.ErrorTypes.Error)
							return exceptionList;
					}

					// Свойства с типом Дата везде обрабатываются одинаково, поэтому можно преобразовать в общем коде.
					if (property.PropertyType == typeof(DateTimeOffset?))
					{
						variableForParameters = TransformDateTime((string)variableForParameters, options, exceptionList, logger);
						if (variableForParameters == null && options.IsRequired())
							return exceptionList;
					}

					// Работа с полями-сущностями.
					if (options.Type == PropertyType.Entity || options.Type == PropertyType.EntityWithCreate)
					{
						var entityName = (string)variableForParameters;
						// баг - при поиске необязательного ссылочного объекта, у которого название для поиска является обязателньым - ошибка становится критичной
						// hack: если необязательное поле пустое - пропускаем, нет смысла дальше искать
						if (!options.IsRequired() && string.IsNullOrEmpty(entityName))
						{
							ResultValues.Add(property.Name, null);
							continue;
						}

						// Добавляем поля и значения для поиска или создания сущностей.
						var propertiesForSearch = GetPropertiesForSearch(property.PropertyType, exceptionList, logger);

						if (propertiesForSearch == null)
							propertiesForSearch = new Dictionary<string, string>();

						// Добавляем поле под служебным наименованием и его значение для
						// работы со связанными с импортируемой сущностью другими сущностями в системе
						// (поиск и создание, к примеру, головная организация, регионы, пользователи), чтобы явно можно было определить
						// их наименование и не спутать с наименованием (полем NAME) обрабатываемой в импорте сущности.
						propertiesForSearch.TryAdd(Constants.KeyAttributes.CustomFieldName, entityName);
						// Пробуем найти сущность в системе.
						variableForParameters = MethodCall(property.PropertyType, Constants.EntityActions.FindEntity, propertiesForSearch, this, false, exceptionList, logger);

						// Создаем сущность, если не удалось найти.
						if (options.Type == PropertyType.EntityWithCreate && variableForParameters == null && !string.IsNullOrEmpty(entityName))
							variableForParameters = MethodCall(property.PropertyType, Constants.EntityActions.CreateEntity, propertiesForSearch, this, exceptionList, isBatch, logger);

						if (CheckPropertyNull(options, variableForParameters, Constants.Resources.EmptyProperty, exceptionList, logger) == Constants.ErrorTypes.Error)
							return exceptionList;
					}
				}

				ResultValues.Add(property.Name, variableForParameters);
			}

			// Специфичные преобразования / проверки полей, которые нет возможности унифицировать.
			// Если метод вернул true, значит при проверках была добавлена ошибка, сущность не может быть загружена.
			var hasTransformationErrors = FillProperties(exceptionList, logger);
			if (hasTransformationErrors)
				return exceptionList;

			// Обновление сущности.
			try
			{
				var propertiesForCreate = GetPropertiesForSearch(EntityType, exceptionList, logger);

				if (entity == null)
				{
					isNewEntity = true;
					entity = (IEntityBase)Activator.CreateInstance(EntityType);
				}

				try
				{
					// Заполнение полей
					var newProperties = UpdateProperties(entity);

					// Создаём анонимный объект с нужной структурой
					var data = new
					{
						structureJson = new
						{
							RelationName = newProperties[0],
							SourceName = newProperties[1],
							SourceCode = newProperties[2],
							TargetName = newProperties[3],
							TargetCode = newProperties[4]
						}
					};

					var options = new JsonSerializerOptions
					{
						Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,  // Главное изменение!
						WriteIndented = true  // Если хотите форматированный вывод (с отступами)
					};

					// Сериализуем в JSON
					var json = JsonSerializer.Serialize(data, options);

					// По идеи тут должен быть вызов функции интеграции.
					// Формируем строку "логин:пароль"
					string credentials = $"{Program.IntegrationLogin}:{Program.IntegrationPassword}";

					// Кодируем в Base64
					string base64Credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials)
					);

					var client = new HttpClient();
					var request = new HttpRequestMessage(HttpMethod.Post, Program.IntegrationUrl + "/CMDB/CreateCIRelation");
					request.Headers.Add("Authorization", $"Basic {base64Credentials}");

					request.Content = new StringContent(json);
					request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

					var response = client.SendAsync(request);

					string valueJson = response.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
					if ((int)response.Result.StatusCode != 500)
					{
						var template = new { value = "" };
						try
						{
							var resultJson = (dynamic)JsonSerializer.Deserialize(response.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult(), template.GetType());
							valueJson = resultJson.value;
						}
						catch (Exception ex)
						{
							// Если не получилось диссириализовать, то далее будет происходить работа с простым ответом.
						}
					}

					if (valueJson == "Связь уже существует")
						exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Warn, Message = valueJson });
					else if (valueJson != "Успешно" || (int)response.Result.StatusCode == 500)
						exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = valueJson });
				}
				catch (Exception ex)
				{
					exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = ex.Message });
				}
			}
			catch (Exception ex)
			{
				exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = ex.Message });

				return exceptionList;
			}

			return exceptionList;
		}

		/// <summary>
		/// Заполнение /обновление полей сущности.
		/// </summary>
		/// <param name="entity">Сущность RX для заполнения.</param>
		private List<string> UpdateProperties(IEntityBase entity)
		{
			var properties = new List<string>();
			var entityProperties = EntityType.GetProperties();
			foreach (var property in entityProperties)
			{
				if (ResultValues.ContainsKey(property.Name))
				{
					var options = BusinessLogic.GetPropertyOptions(property);
					if (options?.Characters == AdditionalCharacters.Collection)
						continue;

					if (property.PropertyType == typeof(double))
					{
						if (string.IsNullOrWhiteSpace(ResultValues[property.Name].ToString()))
							property.SetValue(entity, 0d);
						else
							property.SetValue(entity, Convert.ToDouble(ResultValues[property.Name], CultureInfo.InvariantCulture));
					}
					else
						property.SetValue(entity, ResultValues[property.Name]);


					var propertyName = property.Name ?? string.Empty;
					var propertyValue = string.Empty;
					if (!string.IsNullOrEmpty(property.Name))
						propertyValue = ResultValues[property.Name] != null ? ResultValues[property.Name].ToString() : string.Empty;

					properties.Add(propertyValue);
				}
			}

			return properties;
		}

		/// <summary>
		/// Проверить свойство на пустоту и обработать ошибки.
		/// </summary>
		/// <param name="options">Атрибуты свойства.</param>
		/// <param name="value">Значение свойства.</param>
		/// <param name="message">Текст сообщения при ошибке.</param>
		/// <param name="exceptionList">Список ошибок.</param>
		/// <param name="logger">Логировщик.</param>
		/// <returns>Тип ошибки: ошибка, предупреждение или Debug, если ошибок нет.</returns>
		private string CheckPropertyNull(PropertyOptions options, object value, string message, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
		{
			string errorType = Constants.ErrorTypes.Debug;
			if (value == null || (value is string && string.IsNullOrEmpty((string)value)))
			{
				if (options.IsRequired())
					errorType = BusinessLogic.GetErrorResult(exceptionList, logger, message, options.ExcelName);
				else
					errorType = BusinessLogic.GetWarnResult(exceptionList, logger, message, options.ExcelName);
			}

			return errorType;
		}
	}
}
