using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportData
{
    class Constants
    {
        public class RolesGuides
        {
            public static readonly Guid RoleContractResponsible = new Guid("25C48B40-6111-4283-A94E-7D50E68DECC1");
            public static readonly Guid RoleIncomingDocumentsResponsible = new Guid("63EBE616-8780-4CBB-9AF7-C16251B38A84");
            public static readonly Guid RoleOutgoingDocumentsResponsible = new Guid("372D8FDB-316E-4F3C-9F6D-C2C1292BBFAE");
        }

        public class ErrorTypes
        {
            public const string Error = "Error";
            public const string Warn = "Warn";
            public const string Debug = "Debug";
        }

        public class SheetNames
        {
            public const string BusinessUnits = "НашиОрганизации";
            public const string Departments = "Подразделения";
            public const string Employees = "Сотрудники";
            public const string Companies = "Контрагенты";
            public const string Persons = "Персоны";
            public const string Contracts = "Договоры";
            public const string SupAgreements = "Доп.Соглашения";
            public const string IncomingLetters = "ВходящиеПисьма";
            public const string OutgoingLetters = "ИсходящиеПисьма";
            public const string Orders = "Приказы и Распоряжения";
            public const string Addendums = "Приложения";
            public const string IncomingInvoice = "Вх.Счет";
            public const string DocumentRegister = "ЖурналыРег";
        }

        public class Actions
        {
            public const string ImportCompany = "importcompany";
            public const string ImportCompanies = "importcompanies";
            public const string ImportPersons = "importpersons";
            public const string ImportContracts = "importcontracts";
            public const string ImportSupAgreements = "importsupagreements";
            public const string ImportIncomingLetters = "importincomingletters";
            public const string ImportOutgoingLetters = "importoutgoingletters";
            public const string ImportOrders = "importorders";
            public const string ImportAddendums = "importaddendums";
            public const string ImportIncomingInvoice = "importincominginvoice";
            public const string ImportDocumentRegister = "importdocumentregister";

            public static Dictionary<string, string> dictActions = new Dictionary<string, string>
      {
        {ImportCompany, ImportCompany},
        {ImportCompanies, ImportCompanies},
        {ImportPersons, ImportPersons},
        {ImportContracts, ImportContracts},
        {ImportSupAgreements, ImportSupAgreements},
        {ImportIncomingLetters, ImportIncomingLetters},
        {ImportOutgoingLetters, ImportOutgoingLetters},
        {ImportOrders, ImportOrders},
        {ImportAddendums, ImportAddendums},
        {ImportIncomingInvoice, ImportIncomingInvoice},
        {ImportDocumentRegister, ImportDocumentRegister}
            };
        }

        public class Resources
        {
            public const string IncorrecPsrnLength = "ОГРН должен содержать 13 или 15 цифр.";
            public const string IncorrecTrrcLength = "КПП должен содержать 9 цифр.";
            public const string IncorrecCodeDepartmentLength = "Код подраздленения не должен содержать больше 10 цифр.";
            public const string NotOnlyDigitsTin = "Введите ИНН, состоящий только из цифр.";
            public const string CompanyIncorrectTinLength = "Введите правильное число цифр в ИНН, для организаций - 10, для ИП - 12.";
            public const string PeopleIncorrectTinLength = "Введите правильное число цифр в ИНН, для физических лиц - 12.";
            public const string NotValidTinRegionCode = "Введите ИНН с корректным кодом региона";
            public const string NotValidTin = "Введите ИНН с корректной контрольной цифрой.";
        }

        public class ConfigServices
        {
            public const string IntegrationServiceUrlParamName = "INTEGRATION_SERVICE_URL";
            public const string RequestTimeoutParamName = "INTEGRATION_SERVICE_REQUEST_TIMEOUT";
        }

        public static readonly DateTimeOffset defaultDateTime = new DateTime(1970, 1, 1);
        public const string ignoreDuplicates = "ignore_duplicates";
        public static Dictionary<string, string> dictIgnoreDuplicates = new Dictionary<string, string>
    {
      { ignoreDuplicates, ignoreDuplicates}
        };
    }
}
