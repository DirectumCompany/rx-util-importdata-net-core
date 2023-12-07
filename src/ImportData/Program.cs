using ImportData.IntegrationServicesClient;
using System;
using System.Collections.Generic;
using System.IO;
using NDesk.Options;
using NLog;
using ImportData.Entities.Databooks;
using ImportData.IntegrationServicesClient.Exceptions;
using ImportData.IntegrationServicesClient.Models;
using ImportData.Logic;

namespace ImportData
{
    public class Program
  {
    public static Logger logger = LogManager.GetCurrentClassLogger();
    private const string DefaultConfigSettingsName = @"_ConfigSettings.xml";

    /// <summary>
    /// Выполнение импорта в соответствии с требуемым действием.
    /// </summary>
    /// <param name="action">Действие.</param>
    /// <param name="xlsxPath">Входной файл.</param>
    /// <param name="extraParameters">Дополнительные параметры.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Соответствующий тип сущности.</returns>
    static void ProcessByAction(string action, string xlsxPath, Dictionary<string, string> extraParameters, NLog.Logger logger)
    {
      switch (action)
      {
        case "importcompany":
          logger.Info("Импорт сотрудников");
          logger.Info("-------------");
          EntityProcessor.Process(typeof(Employee), xlsxPath, Constants.SheetNames.Employees, extraParameters, logger);
          logger.Info("Импорт НОР");
          logger.Info("-------------");
          EntityProcessor.Process(typeof(BusinessUnit), xlsxPath, Constants.SheetNames.BusinessUnits, extraParameters, logger);
          logger.Info("Импорт подразделений");
          logger.Info("-------------");
          EntityProcessor.Process(typeof(Department), xlsxPath, Constants.SheetNames.Departments, extraParameters, logger);
          break;
				case "importemployees":
					EntityProcessor.Process(typeof(Employee), xlsxPath, Constants.SheetNames.Employees, extraParameters, logger);
					break;
				case "importdepartments":
					EntityProcessor.Process(typeof(Department), xlsxPath, Constants.SheetNames.Departments, extraParameters, logger);
					break;
				case "importbusinessunits":
					EntityProcessor.Process(typeof(BusinessUnit), xlsxPath, Constants.SheetNames.BusinessUnits, extraParameters, logger);
					break;
				case "importcompanies":
          EntityProcessor.Process(typeof(Company), xlsxPath, Constants.SheetNames.Companies, extraParameters, logger);
          break;
        case "importpersons":
          EntityProcessor.Process(typeof(Person), xlsxPath, Constants.SheetNames.Persons, extraParameters, logger);
          break;
        case "importcontracts":
          EntityProcessor.Process(typeof(Contract), xlsxPath, Constants.SheetNames.Contracts, extraParameters, logger);
          break;
        case "importsupagreements":
          EntityProcessor.Process(typeof(SupAgreement), xlsxPath, Constants.SheetNames.SupAgreements, extraParameters, logger);
          break;
        case "importincomingletters":
          EntityProcessor.Process(typeof(IncomingLetter), xlsxPath, Constants.SheetNames.IncomingLetters, extraParameters, logger);
          break;
        case "importoutgoingletters":
          EntityProcessor.Process(typeof(OutgoingLetter), xlsxPath, Constants.SheetNames.OutgoingLetters, extraParameters, logger);
          break;
        case "importoutgoinglettersaddressees":
          EntityProcessor.Process(typeof(OutgoingLetterAddressees), xlsxPath, Constants.SheetNames.OutgoingLettersAddressees, extraParameters, logger);
          break;
        case "importorders":
          EntityProcessor.Process(typeof(Order), xlsxPath, Constants.SheetNames.Orders, extraParameters, logger);
          break;
        case "importaddendums":
          EntityProcessor.Process(typeof(Addendum), xlsxPath, Constants.SheetNames.Addendums, extraParameters, logger);
          break;
        case "importcontacts":
          EntityProcessor.Process(typeof(Contact), xlsxPath, Constants.SheetNames.Contact, extraParameters, logger);
          break;
        case "importlogins":
          EntityProcessor.Process(typeof(Login), xlsxPath, Constants.SheetNames.Logins, extraParameters, logger);
          break;
        case "importsubstitutions":
          EntityProcessor.Process(typeof(Substitution), xlsxPath, Constants.SheetNames.Substitutions, extraParameters, logger);
          break;
        case "importcompanydirectives":
          EntityProcessor.Process(typeof(CompanyDirective), xlsxPath, Constants.SheetNames.CompanyDirectives, extraParameters, logger);
          break;
        case "importcasefiles":
          EntityProcessor.Process(typeof(CaseFile), xlsxPath, Constants.SheetNames.CaseFiles, extraParameters, logger);
          break;
        default:
          break;
      }
    }

    public static void Main(string[] args)
		{
     	//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importemployees", "-f", $@"csv/Сотрудники.csv" };
			//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importbusinessunits", "-f", $@"csv/НашиОрганизации.csv" };
			//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importdepartments", "-f", $@"csv/Подразделения.csv" };
			//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importcompanies", "-f", $@csv/"Организации.csv" };
			//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importpersons", "-f", $@"csv/Персоны.csv" };
			//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importcontracts", "-f", $@"csv/Договоры.csv" };
			//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importsupagreements", "-f", $@"csv/Допсоглашения.csv" };
			//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importincomingletters", "-f", $@"csv/Входящие.csv" };
			//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importoutgoingletters", "-f", $@"csv/Исходящие.csv" };
			//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importoutgoinglettersaddressees", "-f", $@"csv/Табличная часть исходящих писем.csv" };
			//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importorders", "-f", $@"csv/Приказы.csv" };
			//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importaddendums", "-f", $@"csv/Приложения.csv" };
			//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importcontacts", "-f", $@"csv/Контактные лица.csv" };
			//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importlogins", "-f", $@"csv/Логины.csv" };
			//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importcompanydirectives", "-f", $@"csv/Распоряжения.csv" };
			//args = new[] { "-n", "Administrator", "-p", "11111", "-if", "csv", "-a", "importcasefiles", "-f", $@"csv/Номенклатура дел.csv" };
			logger.Info("=========================== Process Start ===========================");
      var watch = System.Diagnostics.Stopwatch.StartNew();

      #region Обработка параметров.

      var login = string.Empty;
      var password = string.Empty;
      var xlsxPath = string.Empty;
      var action = string.Empty;
      var extraParameters = new Dictionary<string, string>();

      bool isHelp = false;

      var p = new OptionSet() {
                { "n|name=",  "Имя учетной записи DirectumRX.", v => login = v },
                { "p|password=",  "Пароль учетной записи DirectumRX.", v => password = v },
                { "a|action=",  "Действие.", v => action = v },
                { "f|file=",  "Файл с исходными данными.", v => xlsxPath = v },
                { "dr|doc_register_id=",  "Журнал регистрации.", v => extraParameters.Add(Constants.ExtraParameters.DocRegisterId, v)},
                { "d|search_doubles=", "Признак поиска дублей сущностей.", d => extraParameters.Add(Constants.ExtraParameters.IgnoreDuplicates, d)},
                { "ub|update_body=", "Признак обновления последней версии документа.", t => extraParameters.Add(Constants.ExtraParameters.UpdateBody, t) },
                { "if|input_format=", "Формат загружаемого документа.", f => extraParameters.Add(Constants.ExtraParameters.InputFormat, f) },
								{ "csvd|csv_delimiter=", "Разделитель для формата csv.", f => extraParameters.Add(Constants.ExtraParameters.CsvDelimiter, f) },
								{ "h|help", "Show this help", v => isHelp = (v != null) },
      };

      try
      {
        p.Parse(args);
      }
      catch (OptionException e)
      {
        Console.WriteLine("Invalid arguments: " + e.Message);
        p.WriteOptionDescriptions(Console.Out);
        return;
      }

      if (isHelp || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(action) || string.IsNullOrEmpty(xlsxPath))
        if (isHelp || string.IsNullOrEmpty(action) || string.IsNullOrEmpty(xlsxPath))
        {
          p.WriteOptionDescriptions(Console.Out);
          return;
        }

      #endregion

      try
      {
        if (!Constants.Actions.dictActions.ContainsKey(action.ToLower()))
        {
          var message = $"Не найдено действие \"{action}\". Введите действие корректно.";
          throw new Exception(message);
        }

        try
        {
          #region Аутентификация.
          ConfigSettingsService.SetSourcePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultConfigSettingsName));
          Client.Setup(login, password, logger);
          ConfigSettingsService.CheckConnectionToService(login, logger);
          #endregion

          #region Выполнение импорта сущностей.
          ProcessByAction(action.ToLower(), xlsxPath, extraParameters, logger);
          #endregion
        }
        catch (WellKnownKeyNotFoundException ex)
        {
            string message = string.Format("Не найден параметр {0}. Проверьте соответствующую колонку.", ex.Key);
            logger.Error(message);
        }
        catch (Exception ex)
        {
          logger.Error(ex.Message);
        }

      }
      catch (Exception ex)
      {
        logger.Error(ex.Message);
      }
      finally
      {
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        logger.Info($"Всего времени затрачено: {elapsedMs} мс");
        logger.Info("=========================== Process Stop ===========================");
      }
    }
  }
}
