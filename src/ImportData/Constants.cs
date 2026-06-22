using DocumentFormat.OpenXml.Wordprocessing;
using ImportData.IntegrationServicesClient.Models;
using Microsoft.Data.Edm.Library;
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

    public class KeyAttributes
    {
      public const string CustomFieldName = "CustomName";
      public const string Addressee = "Addressee";
      public const string Assignee = "Assignee";
      public const string Bank = "Bank";
      public const string BusinessUnit = "BusinessUnit";
      public const string CEO = "CEO";
      public const string City = "City";
      public const string Code = "Code";
      public const string Correspondent = "Correspondent";
      public const string Counterparty = "Counterparty";
      public const string Company = "Company";
      public const string Created = "Created";
      public const string Currency = "Currency";
      public const string DateOfBirth = "DateOfBirth";
      public const string DeliveryMethod = "DeliveryMethod";
      public const string Department = "Department";
      public const string DocumentDate = "DocumentDate";
      public const string DocumentGroup = "DocumentGroup";
      public const string DocumentKind = "DocumentKind";
      public const string DocumentRegister = "DocumentRegister";
      public const string EndDate = "EndDate";
      public const string Email = "Email";
      public const string FirstName = "FirstName";
      public const string FirstNameRu = "Имя";
      public const string HeadCompany = "HeadCompany";
      public const string HeadOffice = "HeadOffice";
      public const string Index = "Index";
      public const string LastName = "LastName";
      public const string LastNameRu = "Фамилия";
      public const string LeadingDocument = "LeadingDocument";
      public const string LifeCycleState = "LifeCycleState";
      public const string LoginName = "LoginName";
      public const string Password = "Пароль";
      public const string LongTerm = "LongTerm";
      public const string Manager = "Manager";
      public const string ManyAddresses = "IsManyAddressees";
      public const string MiddleName = "MiddleName";
      public const string MiddleNameRu = "Отчество";
      public const string Name = "Name";
      public const string NeedChangePassword = "NeedChangePassword";
      public const string NeedNotifyExpiredAssignments = "NeedNotifyExpiredAssignments";
      public const string NeedNotifyNewAssignments = "NeedNotifyNewAssignments";
      public const string NeedNotifyAssignmentsSummary = "NeedNotifyAssignmentsSummary";
      public const string Nonresident = "Nonresident";
      public const string OurSignatory = "OurSignatory";
      public const string OutgoingDocumentBase = "OutgoingDocumentBase";
      public const string Person = "Person";
      public const string Phones = "Phones";
      public const string PreparedBy = "PreparedBy";
      public const string Region = "Region";
      public const string RegistrationDate = "RegistrationDate";
      public const string RegistrationNumber = "RegistrationNumber";
      public const string RegistrationState = "RegistrationState";
      public const string RegistrationGroup = "RegistrationGroup";
      public const string Responsible = "Responsible";
      public const string ResponsibleEmployee = "ResponsibleEmployee";
      public const string RetentionPeriod = "RetentionPeriod";
      public const string Sex = "Sex";
      public const string StartDate = "StartDate";
      public const string Status = "Status";
      public const string Subject = "Subject";
      public const string Substitute = "Substitute";
      public const string Title = "Title";
      public const string TotalAmount = "TotalAmount";
      public const string TypeAuthentication = "TypeAuthentication";
      public const string User = "User";
      public const string TIN = "TIN";
      public const string TRRC = "TRRC";
      public const string PSRN = "PSRN";
      public const string NCEO = "NCEO";
      public const string NCEA = "NCEA";
      public const string OpenByDefaultForReading = "OpenByDefaultForReading";
      public const string MonitoringType = "MonitoringType";
      public const string RegisterType = "RegisterType";
      public const string DocumentFlow = "DocumentFlow";
      public const string NumberingSection = "NumberingSection";
      public const string NumberingPeriod = "NumberingPeriod";
      public const string NumberOfDigitsInNumber = "NumberOfDigitsInNumber";
      public const string NumberingType = "NumberingType";
      public const string DeadlineInDays = "DeadlineInDays";
      public const string DeadlineInHours = "DeadlineInHours";
      public const string Extension = "Extension";
      public const string DocumentKinds = "DocumentKinds";
      public const string RecipientLinks = "RecipientLinks";
      public const string RequestType = "RequestType";
      public const string Urgency = "Urgency";
      public const string Influence = "Influence";
      public const string ServiceManager = "ServiceManager";
      public const string CategoriesCollection = "CategoriesCollection";
      public const string AvailableFor = "AvailableFor";
      public const string AvailableCollection = "AvailableCollection";
      public const string LogoRu = "Логотип";
      public const string SubsidiaryCategory = "Родительская категория";
      public const string IntentExamplesForTool = "IntentExamplesForTool";
      public const string Example = "Example";
      public const string ConfigurationItemCategoryGuid = "ConfigurationItemCategoryGuid";
      public const string ConfigurationItemCategory = "ConfigurationItemCategory";
      public const string SourceConfigurationItemCategory = "SourceConfigurationItemCategory";
      public const string SourceConfigurationItemKind = "SourceConfigurationItemKind";
      public const string TargetConfigurationItemCategory = "TargetConfigurationItemCategory";
      public const string TargetConfigurationItemKind = "TargetConfigurationItemKind";
      public const string ConfigurationItemKind = "ConfigurationItemKind";
      public const string ConfigurationItemLifeCycleState = "ConfigurationItemLifeCycleState";
      public const string IsClosing = "IsClosing";
      public const string AssignedEmployee = "AssignedEmployee";
      public const string Manufacturer = "Manufacturer";
      public const string SerialNumber = "SerialNumber";
      public const string Version = "Version";
      public const string Note = "Note";
      public const string LocationsAddress = "LocationsAddress";
      public const string ContactInfo = "ContactInfo";
      public const string HardwareRequirements = "HardwareRequirements";
      public const string License = "License";
      public const string Model = "Model";
      public const string PowerConsumption = "PowerConsumption";
      public const string IPAddress = "IPAddress";
      public const string MACAddress = "MACAddress";
      public const string PortNumber = "PortNumber";
      public const string PortSpeed = "PortSpeed";
      public const string SourceName = "SourceName";
      public const string TargetName = "TargetName";
      public const string SourceCode = "SourceCode";
      public const string TargetCode = "TargetCode";
      public const string RelationName = "RelationName";
    }

    public static Dictionary<string, string> AttributeValue = new Dictionary<string, string>
      {
        {KeyAttributes.Status, "Active"},
        {KeyAttributes.TypeAuthentication, "Password"},
        {KeyAttributes.RegisterType, "Registration"},
      };

    public class EntityActions
    {
      public const string CreateEntity = "CreateEntity";
      public const string FindEntity = "FindEntity";
      public const string CreateOrUpdate = "CreateOrUpdate";
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
      public const string Countries = "Страны";
      public const string Currencies = "Валюты";
      public const string Roles = "Роли";
      public const string AssociatedApplications = "Приложения";
      public const string ContractCategories = "КатегорииДоговоров";
      public const string DocumentRegisters = "ЖурналыРегистрации";
      public const string DocumentKinds = "ВидыДокументов";
      public const string Waybill = "Накладные";
      public const string UTD = "УПД";
      public const string ContractStatement = "Акты";
      public const string SimpleDocument = "ПростойДокумент";
      public const string Services = "Услуги";
      public const string ServiceCategories = "КатегорииУслуг";
      public const string ConfigurationItemKind = "Типы КЕ";
      public const string ConfigurationItemRelationType = "Типы связей КЕ";
      public const string RelationsCMDB = "Связи между КЕ";
      public const string ConfigurationItemLifeCycle = "Состояние КЕ";
      public const string ConfigurationItemOfficeEquipment = "КЕ Офисное оборудование";
      public const string ConfigurationItemSoftware = "КЕ Программное обеспечение";
      public const string ConfigurationItemServices = "КЕ Сервис";
      public const string ConfigurationItemLocations = "КЕ Местоположения";
      public const string ConfigurationItemITEquipment = "КЕ ИТ-оборудование";
      public const string ConfigurationItemPCComponents = "КЕ Комплектующие ПК";
      public const string ConfigurationItemTechnicalEquipment = "КЕ Техническое оборудование";
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
      public const string ImportCountries = "importсountries";
      public const string ImportCurrencies = "importcurrencies";
      public const string ImportAssociatedApplications = "importassociatedapplications";
      public const string ImportContractCategories = "importcontractcategories";
      public const string ImportDocumentRegisters = "importdocumentregisters";
      public const string ImportDocumentKinds = "importdocumentkinds";
      public const string ImportRoles = "importroles";
      public const string ImportWaybills = "importwaybills";
      public const string ImportUTD = "importutd";
      public const string ImportContractStatement = "importcontractstatement";
      public const string ImportSimpleDocument = "importsimpledocument";
      public const string ImportServices = "importservices";
      public const string ImportServiceCategories = "importservicecategories";
      public const string ImportCMDB = "importcmdb";

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
        {ImportCountries, ImportCountries},
        {ImportCurrencies, ImportCurrencies},
        {ImportSubstitutions, ImportSubstitutions},
        {ImportRoles, ImportRoles},
        {ImportAssociatedApplications, ImportAssociatedApplications},
        {ImportContractCategories, ImportContractCategories},
        {ImportDocumentRegisters, ImportDocumentRegisters},
        {ImportDocumentKinds, ImportDocumentKinds},
        {ImportWaybills, ImportWaybills},
        {ImportUTD, ImportUTD},
        {ImportContractStatement, ImportContractStatement},
        {ImportSimpleDocument, ImportSimpleDocument},
        {ImportServices, ImportServices},
        {ImportServiceCategories, ImportServiceCategories},
        {ImportCMDB, ImportCMDB},

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
      public const string EmptyColumn = "Не заполнено поле {0}.";
      public const string EmptyProperty = "Не найдено значение для свойства {0}.";
      public const string ErrorFindEmployee = "Не удалось найти в системе соответствующего сотрудника \"{0}\".";
      public const string FileNotExist = "Не найден файл по пути {0}.";
      public const string NeedRequiredDocumentBody = "Импортирумая сущность должна содержать ссылку на тело документа, столбец с наименованием {0}.";
      public const string EntityNotLoaded = "Сущность {0} не загружена, т.к. уже существует в системе.";
      public const string RegistrationGroupName = "Группа регистрации";
      public const string EmailNotValid = "Введенный адрес электронной почты имеет неверный формат.";
    }

    public class ConfigServices
    {
      public const string IntegrationServiceUrlParamName = "INTEGRATION_SERVICE_URL";
      public const string RequestTimeoutParamName = "INTEGRATION_SERVICE_REQUEST_TIMEOUT";
      public const string BatchRequestsCountParamName = "INTEGRATION_SERVICE_BATCH_REQUESTS_COUNT";
    }

    public const string ignoreDuplicates = "ignore";
    public const string CellNameFile = "Файл";
    public static Dictionary<string, string> dictIgnoreDuplicates = new Dictionary<string, string>
    {
      { ignoreDuplicates, ignoreDuplicates}
    };

  }

  public enum RequiredType
  {
    NotRequired = 0,
    Required = 1
  }

  public enum PropertyType
  {
    Simple = 0,
    Entity = 1,
    EntityWithCreate = 2
  }

  public enum AdditionalCharacters
  {
    Default = 0,
    ForSearch = 1,
    CreateFromOtherProperties = 2,
    Collection = 3
  }
}
