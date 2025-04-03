using ImportData;
using Xunit.Extensions.Ordering;

namespace Tests.Databooks
{
    public partial class Tests
    {
        [Fact, Order(30)]
        public void T3_PersonImport()
        {
            var xlsxPath = TestSettings.PersonsPathXlsx;
            var action = ImportData.Constants.Actions.ImportPersons;
            var sheetName = ImportData.Constants.SheetNames.Persons;

            Program.Main(Common.GetArgs(action, xlsxPath));

            var errorList = new List<string>();
            foreach (var expectedPerson in Common.XlsxParse(xlsxPath, sheetName))
            {
                var error = EqualsPerson(expectedPerson);

                if (string.IsNullOrEmpty(error))
                    continue;

                errorList.Add(error);
            }
            if (errorList.Any())
                Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, errorList));
        }

        public static string EqualsPerson(List<string> parameters, int shift = 0)
        {
            var exceptionList = new List<Structures.ExceptionsStruct>();
            var name = string.Join(' ', parameters[shift + 0].Trim(), parameters[shift + 1].Trim(), parameters[shift + 2].Trim()).Trim();
            var actualPerson = BusinessLogic.GetEntityWithFilter<IPersons>(x => x.Name == name, exceptionList, TestSettings.Logger, true);

            if (actualPerson == null)
                return $"Не найдена персона {name}";

            var errorList = new List<string>
            {
                Common.CheckParam(actualPerson.LastName, parameters[shift + 0], "LastName"),
                Common.CheckParam(actualPerson.FirstName, parameters[shift + 1], "FirstName"),
                Common.CheckParam(actualPerson.MiddleName, parameters[shift + 2], "MiddleName"),
                Common.CheckParam(actualPerson.Sex, BusinessLogic.GetPropertySex(parameters[shift + 3]), "Sex"),
                Common.CheckParam(actualPerson.DateOfBirth, parameters[shift + 4], "DateOfBirth"),
                Common.CheckParam(actualPerson.TIN, parameters[shift + 5], "TIN"),
                Common.CheckParam(actualPerson.INILA, parameters[shift + 6], "INILA"),
                Common.CheckParam(actualPerson.City, parameters[shift + 7], "City"),
                Common.CheckParam(actualPerson.Region, parameters[shift + 8], "Region"),
                Common.CheckParam(actualPerson.LegalAddress, parameters[shift + 9], "LegalAddress"),
                Common.CheckParam(actualPerson.PostalAddress, parameters[shift + 10], "PostalAddress"),
                Common.CheckParam(actualPerson.Phones, parameters[shift + 11], "Phones"),
                Common.CheckParam(actualPerson.Email, parameters[shift + 12], "Email"),
                Common.CheckParam(actualPerson.Homepage, parameters[shift + 13], "Homepage"),
                Common.CheckParam(actualPerson.Bank, parameters[shift + 14], "Bank"),
                Common.CheckParam(actualPerson.Account, parameters[shift + 15], "Account"),
                Common.CheckParam(actualPerson.Note, parameters[shift + 16], "Note")
            };

            errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (errorList.Any())
                errorList.Insert(0, $"Ошибка в сущности: {name}");

            return string.Join(Environment.NewLine, errorList);
        }
    }
}