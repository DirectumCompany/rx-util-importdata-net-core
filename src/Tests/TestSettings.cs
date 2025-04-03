using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Tests
{
    internal static class TestSettings
    {
        public static Logger Logger = LogManager.GetCurrentClassLogger();

        public const string Login = "Administrator";
        public const string Password = "11111";
        public const string XlsxFolderPath = "Templates";
       
        public const string CompanyPathXlsx = $@"{XlsxFolderPath}\Company.xlsx";
        public const string CompaniesPathXlsx = $@"{XlsxFolderPath}\Companies.xlsx";
        public const string PersonsPathXlsx = $@"{XlsxFolderPath}\Persons.xlsx";
        public const string LoginsPathXlsx = $@"{XlsxFolderPath}\Logins.xlsx";
        public const string ContactsPathXlsx = $@"{XlsxFolderPath}\Contacts.xlsx";
        public const string CaseFilePathXlsx = $@"{XlsxFolderPath}\CaseFile.xlsx";
        public const string ContractsPathXlsx = $@"{XlsxFolderPath}\Contracts.xlsx";
        public const string AddendumsPathXlsx = $@"{XlsxFolderPath}\Supagreements.xlsx";
        public const string SupagreementsPathXlsx = $@"{XlsxFolderPath}\Supagreements.xlsx";
        public const string IncomingLettersPathXlsx = $@"{XlsxFolderPath}\IncomingLetters.xlsx";
        public const string OutgoingLettersPathXlsx = $@"{XlsxFolderPath}\OutgoingLetters.xlsx";
        public const string OutgoingLettersAddresseesPathXlsx = $@"{XlsxFolderPath}\OutgoingLettersAddressees.xlsx";
        public const string OrdersPathXlsx = $@"{XlsxFolderPath}\Orders.xlsx";
        public const string CompanyDirectivesPathXlsx = $@"{XlsxFolderPath}\CompanyDirectives.xlsx";
        public const string SimpleDocumentPathXlsx = $@"{XlsxFolderPath}\SimpleDocument.xlsx";

    }
}
