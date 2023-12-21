using DocumentFormat.OpenXml.Bibliography;
using System.Security.Cryptography;
using ImportData;
using ImportData.IntegrationServicesClient.Models;
using Xunit.Extensions.Ordering;
using DocumentFormat.OpenXml.Office2010.Word;

namespace Tests.EDocs
{
    public partial class Tests
    {
        [Fact, Order(80)]
        public void T8_CompanyDirectivesImport()
        {
            var xlsxPath = TestSettings.CompanyDirectivesPathXlsx;
            var action = ImportData.Constants.Actions.ImportCompanyDirectives;
            var sheetName = ImportData.Constants.SheetNames.CompanyDirectives;

            var items = Common.XlsxParse(xlsxPath, sheetName);

            Program.Main(Common.GetArgs(action, xlsxPath));

            var errorList = new List<string>();
            foreach (var expectedCompanyDirective in items)
            {
                var error = EqualsCompanyDirectives(expectedCompanyDirective);

                if (string.IsNullOrEmpty(error))
                    continue;

                errorList.Add(error);
            }
            if (errorList.Any())
                Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, errorList));
        }

        public static string EqualsCompanyDirectives(List<string> parameters, int shift = 0)
        {
            var exceptionList = new List<Structures.ExceptionsStruct>();

            var actualCompanyDirective = Common.GetOfficialDocument<ICompanyDirective>(parameters[shift + 0], parameters[shift + 1], parameters[shift + 12]);

            var name = Common.GetDocumentName(parameters[shift + 2], parameters[shift + 0], parameters[shift + 1], parameters[shift + 3]);

            if (actualCompanyDirective == null)
                return $"Не найдено распоряжение: {name}";

            var errorList = new List<string>
            {
                Common.CheckParam(actualCompanyDirective.RegistrationNumber, parameters[shift + 0], "RegistrationNumber"),
                Common.CheckParam(actualCompanyDirective.RegistrationDate, parameters[shift + 1], "RegistrationDate"),
                Common.CheckParam(actualCompanyDirective.DocumentKind, parameters[shift + 2], "DocumentKind"),
                Common.CheckParam(actualCompanyDirective.Subject, parameters[shift + 3], "Subject"),
                Common.CheckParam(actualCompanyDirective.BusinessUnit, parameters[shift + 4], "BusinessUnit"),
                Common.CheckParam(actualCompanyDirective.Department, parameters[shift + 5], "Department"),
                Common.CheckParam(actualCompanyDirective.LastVersion(), parameters[shift + 6], "LastVersion"),
                Common.CheckParam(actualCompanyDirective.Assignee, parameters[shift + 7], "Assignee"),
                Common.CheckParam(actualCompanyDirective.PreparedBy, parameters[shift + 8], "PreparedBy"),
                Common.CheckParam(actualCompanyDirective.OurSignatory, parameters[shift + 9], "OurSignatory"),
                Common.CheckParam(actualCompanyDirective.LifeCycleState, BusinessLogic.GetPropertyLifeCycleState(parameters[shift + 10]), "LifeCycleState"),
                Common.CheckParam(actualCompanyDirective.Note, parameters[shift + 11], "Note"),
                Common.CheckParam(actualCompanyDirective.DocumentRegister, parameters[shift + 12], "DocumentRegister"),
                Common.CheckParam(actualCompanyDirective.RegistrationState, BusinessLogic.GetRegistrationsState(parameters[shift + 13]), "RegistrationState")
            };

            errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (errorList.Any())
                errorList.Insert(0, $"Ошибка в сущности:  {name}");

            return string.Join(Environment.NewLine, errorList);
        }
    }
}