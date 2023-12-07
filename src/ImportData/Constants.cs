using System;
using System.Collections.Generic;

namespace ImportData
{
  public class Constants
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
      public const string OutgoingLettersAddressees = "ИсходящиеПисьмаАдресаты";
      public const string Orders = "Приказы";
      public const string Addendums = "Приложения";
      public const string Contact = "Контактные лица";
      public const string CompanyDirectives = "Распоряжения";
      public const string Logins = "Логины";
      public const string Substitutions = "Замещения";
      public const string CaseFiles = "Номенклатура дел";
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
      public const string ImportOutgoingLettersAddressees = "importoutgoinglettersaddressees";
      public const string ImportOrders = "importorders";
      public const string ImportAddendums = "importaddendums";
      public const string ImportDepartments = "importdepartments";
      public const string ImportEmployees = "importemployees";
      public const string ImportContacts = "importcontacts";
      public const string ImportCompanyDirectives = "importcompanydirectives";
      public const string ImportLogins = "importlogins";
      public const string ImportSubstitutions = "importsubstitutions";
      public const string ImportCaseFiles = "importcasefiles";
			public const string ImportBusinessUnits = "importbusinessunits";

			// Инициализация клиента, для тестов.
			public const string InitForTests = "init";

      public static Dictionary<string, string> dictActions = new Dictionary<string, string>
            {
                {ImportCompany, ImportCompany},
                {ImportCompanies, ImportCompanies},
                {ImportPersons, ImportPersons},
                {ImportContracts, ImportContracts},
                {ImportSupAgreements, ImportSupAgreements},
                {ImportIncomingLetters, ImportIncomingLetters},
                {ImportOutgoingLetters, ImportOutgoingLetters},
                {ImportOutgoingLettersAddressees, ImportOutgoingLettersAddressees},
                {ImportOrders, ImportOrders},
                {ImportAddendums, ImportAddendums},
                {ImportDepartments, ImportDepartments},
                {ImportEmployees, ImportEmployees},
                {ImportContacts, ImportContacts},
                {ImportCompanyDirectives, ImportCompanyDirectives},
                {ImportLogins, ImportLogins},
                {ImportCaseFiles, ImportCaseFiles},
                {ImportBusinessUnits, ImportBusinessUnits},

                // Инициализация клиента, для тестов.
                {InitForTests, InitForTests}
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
      public const string NonresidentIncorectTinLength = "Введите правильное число цифр в ИНН, для нерезидента - до 12.";
      public const string NotValidTinRegionCode = "Введите ИНН с корректным кодом региона";
      public const string NotValidTin = "Введите ИНН с корректной контрольной цифрой.";
      public const string IncorrecNceoLength = "ОКПО должен содержать 8 или 10 цифр";
    }

    public class ConfigServices
    {
      public const string IntegrationServiceUrlParamName = "INTEGRATION_SERVICE_URL";
      public const string RequestTimeoutParamName = "INTEGRATION_SERVICE_REQUEST_TIMEOUT";
    }

    public class ExtraParameters
    {
      public const string DocRegisterId = "doc_register_id";
      public const string IgnoreDuplicates = "ignore_duplicates";
      public const string UpdateBody = "update_body";
      public const string InputFormat = "input_format";
      public const string CsvDelimiter = "csv_delimiter";
    }


    public const string ignoreDuplicates = "ignore";
    public static Dictionary<string, string> dictIgnoreDuplicates = new Dictionary<string, string>
    {
      { ignoreDuplicates, ignoreDuplicates}
    };
  }
}
