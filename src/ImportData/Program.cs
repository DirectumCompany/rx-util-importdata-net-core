using ImportData.IntegrationServicesClient;
using System;
using System.Collections.Generic;
using System.IO;
using NDesk.Options;
using NLog;
using ImportData.Entities.Databooks;
using ImportData.IntegrationServicesClient.Exceptions;
using ImportData.IntegrationServicesClient.Models;

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
    static void ProcessByAction(string action, string xlsxPath, Dictionary<string, string> extraParameters, string ignoreDuplicates, bool isBatch, NLog.Logger logger)
    {
      switch (action)
      {
        case "importcompany":
          logger.Info("Импорт персон");
          logger.Info("-------------");
          EntityProcessor.Process(typeof(Person), xlsxPath, Constants.SheetNames.Employees, extraParameters, ignoreDuplicates, isBatch, logger);
          logger.Info("Импорт сотрудников");
          logger.Info("-------------");
          EntityProcessor.Process(typeof(Employee), xlsxPath, Constants.SheetNames.Employees, extraParameters, ignoreDuplicates, isBatch, logger);
          logger.Info("Импорт НОР");
          logger.Info("-------------");
          EntityProcessor.Process(typeof(BusinessUnit), xlsxPath, Constants.SheetNames.BusinessUnits, extraParameters, ignoreDuplicates, isBatch, logger);
          logger.Info("Импорт подразделений");
          logger.Info("-------------");
          EntityProcessor.Process(typeof(Department), xlsxPath, Constants.SheetNames.Departments, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importcompanies":
          EntityProcessor.Process(typeof(Company), xlsxPath, Constants.SheetNames.Companies, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importpersons":
          EntityProcessor.Process(typeof(Person), xlsxPath, Constants.SheetNames.Persons, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importcontracts":
          EntityProcessor.Process(typeof(Contract), xlsxPath, Constants.SheetNames.Contracts, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importsupagreements":
          EntityProcessor.Process(typeof(SupAgreement), xlsxPath, Constants.SheetNames.SupAgreements, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importincomingletters":
          EntityProcessor.Process(typeof(IncomingLetter), xlsxPath, Constants.SheetNames.IncomingLetters, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importoutgoingletters":
          EntityProcessor.Process(typeof(OutgoingLetter), xlsxPath, Constants.SheetNames.OutgoingLetters, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importoutgoinglettersaddressees":
          EntityProcessor.Process(typeof(OutgoingLetterAddressees), xlsxPath, Constants.SheetNames.OutgoingLettersAddressees, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importorders":
          EntityProcessor.Process(typeof(Order), xlsxPath, Constants.SheetNames.Orders, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importaddendums":
          EntityProcessor.Process(typeof(Addendum), xlsxPath, Constants.SheetNames.Addendums, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importdepartments":
          EntityProcessor.Process(typeof(Department), xlsxPath, Constants.SheetNames.Departments, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importemployees":
          EntityProcessor.Process(typeof(Employee), xlsxPath, Constants.SheetNames.Employees, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importcontacts":
          EntityProcessor.Process(typeof(Contact), xlsxPath, Constants.SheetNames.Contact, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importlogins":
          EntityProcessor.Process(typeof(Login), xlsxPath, Constants.SheetNames.Logins, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importsubstitutions":
          EntityProcessor.Process(typeof(Substitution), xlsxPath, Constants.SheetNames.Substitutions, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importcompanydirectives":
          EntityProcessor.Process(typeof(CompanyDirective), xlsxPath, Constants.SheetNames.CompanyDirectives, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importcasefiles":
          EntityProcessor.Process(typeof(CaseFile), xlsxPath, Constants.SheetNames.CaseFiles, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importсountries":
          EntityProcessor.Process(typeof(Country), xlsxPath, Constants.SheetNames.Countries, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importcurrencies":
          EntityProcessor.Process(typeof(Currency), xlsxPath, Constants.SheetNames.Currencies, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importassociatedapplications":
          EntityProcessor.Process(typeof(AssociatedApplication), xlsxPath, Constants.SheetNames.AssociatedApplications, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importcontractcategories":
          EntityProcessor.Process(typeof(ContractCategory), xlsxPath, Constants.SheetNames.ContractCategories, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importdocumentregisters":
          EntityProcessor.Process(typeof(DocumentRegister), xlsxPath, Constants.SheetNames.DocumentRegisters, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importdocumentkinds":
          EntityProcessor.Process(typeof(DocumentKind), xlsxPath, Constants.SheetNames.DocumentKinds, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importroles":
          EntityProcessor.Process(typeof(Role), xlsxPath, Constants.SheetNames.Roles, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        case "importsimpledocument":
          EntityProcessor.Process(typeof(SimpleDocument), xlsxPath, Constants.SheetNames.SimpleDocument, extraParameters, ignoreDuplicates, isBatch, logger);
          break;
        default:
          break;
      }
    }

    public static void Main(string[] args)
    {

      logger.Info("=========================== Process Start ===========================");
      var watch = System.Diagnostics.Stopwatch.StartNew();

      #region Обработка параметров.

      var login = string.Empty;
      var password = string.Empty;
      var xlsxPath = string.Empty;
      var action = string.Empty;
      var extraParameters = new Dictionary<string, string>();
      var ignoreDuplicates = string.Empty;
      var isBatch = false;

      bool isHelp = false;

      var p = new OptionSet() {
                { "n|name=",  "Имя учетной записи DirectumRX.", v => login = v },
                { "p|password=",  "Пароль учетной записи DirectumRX.", v => password = v },
                { "a|action=",  "Действие.", v => action = v },
                { "f|file=",  "Файл с исходными данными.", v => xlsxPath = v },
                { "b|batch", "Создавать сущности пакетными запросами", v => isBatch = v != null},
                { "dr|doc_register_id=",  "Журнал регистрации.", v => extraParameters.Add("doc_register_id", v)},
                { "d|search_doubles=", "Признак поиска дублей сущностей.", d => ignoreDuplicates = d},
                { "ub|update_body=", "Признак обновления последней версии документа.", t => extraParameters.Add("update_body", t) },
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
          ProcessByAction(action.ToLower(), xlsxPath, extraParameters, ignoreDuplicates, isBatch, logger);
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
