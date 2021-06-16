using ImportData.IntegrationServicesClient;
using System;
using System.Collections.Generic;
using System.IO;
using NDesk.Options;
using NLog;

namespace ImportData
{
    class Program
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
        static void ProcessByAction(string action, string xlsxPath, Dictionary<string, string> extraParameters, string ignoreDuplicates, NLog.Logger logger)
        {
            switch (action)
            {
                case "importcompany":
                    logger.Info("Импорт сотрудников");
                    logger.Info("-------------");
                    EntityProcessor.Procces(typeof(Employee), xlsxPath, Constants.SheetNames.Employees, extraParameters, ignoreDuplicates, logger);
                    logger.Info("Импорт НОР");
                    logger.Info("-------------");
                    EntityProcessor.Procces(typeof(BusinessUnit), xlsxPath, Constants.SheetNames.BusinessUnits, extraParameters, ignoreDuplicates, logger);
                    logger.Info("Импорт подразделений");
                    logger.Info("-------------");
                    EntityProcessor.Procces(typeof(Department), xlsxPath, Constants.SheetNames.Departments, extraParameters, ignoreDuplicates, logger);
                    break;
                case "importcompanies":
                    EntityProcessor.Procces(typeof(Company), xlsxPath, Constants.SheetNames.Companies, extraParameters, ignoreDuplicates, logger);
                    break;
                case "importpersons":
                    EntityProcessor.Procces(typeof(Person), xlsxPath, Constants.SheetNames.Persons, extraParameters, ignoreDuplicates, logger);
                    break;
                case "importcontracts":
                    EntityProcessor.Procces(typeof(Contract), xlsxPath, Constants.SheetNames.Contracts, extraParameters, ignoreDuplicates, logger);
                    break;
                case "importsupagreements":
                    EntityProcessor.Procces(typeof(SupAgreement), xlsxPath, Constants.SheetNames.SupAgreements, extraParameters, ignoreDuplicates, logger);
                    break;
                case "importincomingletters":
                    EntityProcessor.Procces(typeof(IncomingLetter), xlsxPath, Constants.SheetNames.IncomingLetters, extraParameters, ignoreDuplicates, logger);
                    break;
                case "importoutgoingletters":
                    EntityProcessor.Procces(typeof(OutgoingLetter), xlsxPath, Constants.SheetNames.OutgoingLetters, extraParameters, ignoreDuplicates, logger);
                    break;
                case "importorders":
                    EntityProcessor.Procces(typeof(Order), xlsxPath, Constants.SheetNames.Orders, extraParameters, ignoreDuplicates, logger);
                    break;
                case "importaddendums":
                    EntityProcessor.Procces(typeof(Addendum), xlsxPath, Constants.SheetNames.Addendums, extraParameters, ignoreDuplicates, logger);
                    break;
                case "importincominginvoice":
                    EntityProcessor.Procces(typeof(IncomingInvoice), xlsxPath, Constants.SheetNames.IncomingInvoice, extraParameters, ignoreDuplicates, logger);
                    break;
                case "importdocumentregister":
                    EntityProcessor.Procces(typeof(DocumentRegister), xlsxPath, Constants.SheetNames.DocumentRegister, extraParameters, ignoreDuplicates, logger);
                    break;
                default:
                    break;
            }
        }

        static void Main(string[] args)
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

            bool isHelp = false;

            var p = new OptionSet() {
                { "n|name=",  "Имя учетной записи DirectumRX.", v => login = v },
                { "p|password=",  "Пароль учетной записи DirectumRX.", v => password = v },
                { "a|action=",  "Действие.", v => action = v },
                { "f|file=",  "Файл с исходными данными.", v => xlsxPath = v },
                { "dr|doc_register_id=",  "Журнал регистрации.", v => extraParameters.Add("doc_register_id", v)},
                { "d|search_doubles=", "Признак поиска дублей сущностей.", d => ignoreDuplicates = d},
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
                    #endregion

                    #region Выполнение импорта сущностей.
                    ProcessByAction(action.ToLower(), xlsxPath, extraParameters, ignoreDuplicates, logger);
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
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
