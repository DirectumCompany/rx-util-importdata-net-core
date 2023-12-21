using ImportData;
using Xunit.Extensions.Ordering;

namespace Tests.Databooks
{
    public partial class Tests
    {
        [Fact, Order(50)]
        public void T5_ContactsImport()
        {
            var xlsxPath = TestSettings.ContactsPathXlsx;
            var action = ImportData.Constants.Actions.ImportContacts;
            var sheetName = ImportData.Constants.SheetNames.Contact;

            Program.Main(Common.GetArgs(action, xlsxPath));

            var errorList = new List<string>();
            foreach (var expectedContact in Common.XlsxParse(xlsxPath, sheetName))
            {
                var error = EqualsContact(expectedContact);

                if (string.IsNullOrEmpty(error))
                    continue;

                errorList.Add(error);
            }
            if (errorList.Any())
                Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, errorList));
        }

        public static string EqualsContact(List<string> parameters, int shift = 0)
        {
            var exceptionList = new List<Structures.ExceptionsStruct>();
            var name = string.Join(' ', parameters[shift + 0].Trim(), parameters[shift + 1].Trim(), parameters[shift + 2].Trim()).Trim();

            var actualContact = BusinessLogic.GetEntityWithFilter<IContacts>(x => x.Name == name, exceptionList, TestSettings.Logger, true);
            if (actualContact == null)
                return $"Не найден контакт {name}";

            var actualPerson = actualContact.Person;
            if (actualPerson == null)
                return $"Не найдена персона {name}";

            var errorList = new List<string>
            {
                Common.CheckParam(actualPerson.LastName, parameters[shift + 0], "LastName"),
                Common.CheckParam(actualPerson.FirstName, parameters[shift + 1], "FirstName"),
                Common.CheckParam(actualPerson.MiddleName, parameters[shift + 2], "MiddleName"),
                Common.CheckParam(actualContact.Company, parameters[shift + 3], "Company"),
                Common.CheckParam(actualContact.JobTitle, parameters[shift + 4], "JobTitle"),
                Common.CheckParam(actualContact.Phone, parameters[shift + 5], "Phone"),
                Common.CheckParam(actualContact.Fax, parameters[shift + 6], "Fax"),
                Common.CheckParam(actualContact.Email, parameters[shift + 7], "Email"),
                Common.CheckParam(actualContact.Homepage, parameters[shift + 8], "Homepage"),
                Common.CheckParam(actualContact.Note, parameters[shift + 9], "Note")
            };

            errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (errorList.Any())
                errorList.Insert(0, $"Ошибка в сущности: {name}");

            return string.Join(Environment.NewLine, errorList);
        }
    }
}