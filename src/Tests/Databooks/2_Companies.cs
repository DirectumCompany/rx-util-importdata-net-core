using ConfigSettings.Patch;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using ImportData;
using Xunit.Extensions.Ordering;

namespace Tests.Databooks
{
    public partial class Tests
    {
        [Fact, Order(20)]
        public void T2_CompaniesImport()
        {
            var xlsxPath = TestSettings.CompaniesPathXlsx;
            var action = ImportData.Constants.Actions.ImportCompanies;
            var sheetName = ImportData.Constants.SheetNames.Companies;

            Program.Main(Common.GetArgs(action, xlsxPath));

            var errorList = new List<string>();
            foreach (var expectedCompany in Common.XlsxParse(xlsxPath, sheetName))
            {
                var error = EqualsCompany(expectedCompany);

                if (string.IsNullOrEmpty(error))
                    continue;

                errorList.Add(error);
            }
            if (errorList.Any())
                Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, errorList));
        }

        public static string EqualsCompany(List<string> parameters, int shift = 0)
        {
            var exceptionList = new List<Structures.ExceptionsStruct>();
            var name = parameters[shift + 0].Trim();
            var actualCompany = BusinessLogic.GetEntityWithFilter<ICompanies>(x => x.Name == name, exceptionList, TestSettings.Logger, true);

            if (actualCompany == null)
                return $"Не найдена компания {name}";

            var errorList = new List<string>
            {
                Common.CheckParam(actualCompany.Name, parameters[shift + 0], "Name"),
                Common.CheckParam(actualCompany.LegalName, parameters[shift + 1], "LegalName"),
                Common.CheckParam(actualCompany.HeadCompany, parameters[shift + 2], "HeadCompany"),
                Common.CheckParam(actualCompany.Nonresident.ToString(), (parameters[shift + 3].Trim().ToLower() == "да").ToString(), "Nonresident"),
                Common.CheckParam(actualCompany.TIN, parameters[shift + 4], "TIN"),
                Common.CheckParam(actualCompany.PSRN, parameters[shift + 5], "PSRN"),
                Common.CheckParam(actualCompany.NCEO, parameters[shift + 6], "NCEO"),
                Common.CheckParam(actualCompany.NCEA, parameters[shift + 7], "NCEA"),
                Common.CheckParam(actualCompany.TRRC, parameters[shift + 8], "TRRC"),
                Common.CheckParam(actualCompany.City?.Name, parameters[shift + 9], "City"),
                //Регион подставляется прикладной по городу, не проверяем.
                //Common.CheckParam(actualCompany.Region, parameters[shift + 10], "Region"),
                Common.CheckParam(actualCompany.LegalAddress, parameters[shift + 11], "LegalAddress"),
                Common.CheckParam(actualCompany.PostalAddress, parameters[shift + 12], "PostalAddress"),
                Common.CheckParam(actualCompany.Phones, parameters[shift + 13], "Phones"),
                Common.CheckParam(actualCompany.Email, parameters[shift + 14], "Email"),
                Common.CheckParam(actualCompany.Homepage, parameters[shift + 15], "Homepage"),
                Common.CheckParam(actualCompany.Note, parameters[shift + 16], "Note"),
                Common.CheckParam(actualCompany.Account, parameters[shift + 17], "Account"),
                Common.CheckParam(actualCompany.Bank?.Name.ToLower(), parameters[shift + 18].ToLower(), "Bank"),
                Common.CheckParam(actualCompany.Responsible, parameters[shift + 19], "Responsible")
            };

            errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (errorList.Any())
                errorList.Insert(0, $"Ошибка в сущности: {name}");

            return string.Join(Environment.NewLine, errorList);
        }
    }
}
