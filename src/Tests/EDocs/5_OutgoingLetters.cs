using DocumentFormat.OpenXml.Bibliography;
using ImportData;
using ImportData.IntegrationServicesClient.Models;
using Xunit.Extensions.Ordering;

namespace Tests.EDocs
{
    public partial class Tests
    {
        [Fact, Order(50)]
        public void T5_OutgoingLettersImport()
        {
            var xlsxPath = TestSettings.OutgoingLettersPathXlsx;
            var action = ImportData.Constants.Actions.ImportOutgoingLetters;
            var sheetName = ImportData.Constants.SheetNames.OutgoingLetters;

            var items = Common.XlsxParse(xlsxPath, sheetName);

            Program.Main(Common.GetArgs(action, xlsxPath));

            var errorList = new List<string>();
            foreach (var expectedOutgoingLetter in items)
            {
                var error = EqualsOutgoingLetter(expectedOutgoingLetter);

                if (string.IsNullOrEmpty(error))
                    continue;

                errorList.Add(error);
            }
            if (errorList.Any())
                Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, errorList));
        }

        public static string EqualsOutgoingLetter(List<string> parameters, int shift = 0)
        {
            var actualOutgoingLetter = Common.GetOfficialDocument<IOutgoingLetters>(parameters[shift + 0], parameters[shift + 1], parameters[shift + 10]);

            var name = Common.GetDocumentName(parameters[shift + 3], parameters[shift + 0], parameters[shift + 1], parameters[shift + 4]);

            if (actualOutgoingLetter == null)
                return $"Не найдено входящее письмо: {name}";

            var errorList = new List<string>
            {
                Common.CheckParam(actualOutgoingLetter.RegistrationNumber, parameters[shift + 0], "RegistrationNumber"),
                Common.CheckParam(actualOutgoingLetter.RegistrationDate, parameters[shift + 1], "RegistrationDate"),
                Common.CheckParam(actualOutgoingLetter.DocumentKind, parameters[shift + 3], "DocumentKind"),
                Common.CheckParam(actualOutgoingLetter.Subject, parameters[shift + 4], "Subject"),
                Common.CheckParam(actualOutgoingLetter.Department, parameters[shift + 5], "Department"),
                Common.CheckParam(actualOutgoingLetter.PreparedBy, parameters[shift + 6], "PreparedBy"),
                Common.CheckParam(actualOutgoingLetter.LastVersion(), parameters[shift + 7], "LastVersion"),
                Common.CheckParam(actualOutgoingLetter.Note, parameters[shift + 8], "Note"),
                Common.CheckParam(actualOutgoingLetter.DeliveryMethod, parameters[shift + 9], "DeliveryMethod"),
                Common.CheckParam(actualOutgoingLetter.DocumentRegister, parameters[shift + 10], "DocumentRegister"),
                Common.CheckParam(actualOutgoingLetter.RegistrationState, BusinessLogic.GetRegistrationsState(parameters[shift + 11]), "RegistrationState"),
            };

            errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (errorList.Any())
                errorList.Insert(0, $"Ошибка в сущности: {name}");

            return string.Join(Environment.NewLine, errorList);
        }
    }
}