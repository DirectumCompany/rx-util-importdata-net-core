using DocumentFormat.OpenXml.Bibliography;
using ImportData;
using ImportData.IntegrationServicesClient.Models;
using Xunit.Extensions.Ordering;

namespace Tests.EDocs
{
    public partial class Tests
    {
        [Fact, Order(40)]
        public void T4_IncomingLettersImport()
        {
            var xlsxPath = TestSettings.IncomingLettersPathXlsx;
            var action = ImportData.Constants.Actions.ImportIncomingLetters;
            var sheetName = ImportData.Constants.SheetNames.IncomingLetters;

            var items = Common.XlsxParse(xlsxPath, sheetName);

            Program.Main(Common.GetArgs(action, xlsxPath));

            var errorList = new List<string>();
            foreach (var expectedIncomingLetter in items)
            {
                var error = EqualsIncomingLetter(expectedIncomingLetter);

                if (string.IsNullOrEmpty(error))
                    continue;

                errorList.Add(error);
            }
            if (errorList.Any())
                Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, errorList));
        }

        public static string EqualsIncomingLetter(List<string> parameters, int shift = 0)
        {
            var actualIncomingLetter = Common.GetOfficialDocument<IIncomingLetters>(parameters[shift + 0], parameters[shift + 1], parameters[shift + 12]);

            var name = Common.GetDocumentName(parameters[shift + 3], parameters[shift + 0], parameters[shift + 1], parameters[shift + 4]);

            if (actualIncomingLetter == null)
                return $"Не найдено входящее письмо: {name}";

            var errorList = new List<string>
            {
                Common.CheckParam(actualIncomingLetter.RegistrationNumber, parameters[shift + 0], "RegistrationNumber"),
                Common.CheckParam(actualIncomingLetter.RegistrationDate, parameters[shift + 1], "RegistrationDate"),
                Common.CheckParam(actualIncomingLetter.Correspondent, parameters[shift + 2], "Correspondent"),
                Common.CheckParam(actualIncomingLetter.DocumentKind, parameters[shift + 3], "DocumentKind"),
                Common.CheckParam(actualIncomingLetter.Subject, parameters[shift + 4], "Subject"),
                Common.CheckParam(actualIncomingLetter.Department, parameters[shift + 5], "Department"),
                Common.CheckParam(actualIncomingLetter.LastVersion(), parameters[shift + 6], "LastVersion"),
                Common.CheckParam(actualIncomingLetter.Dated, parameters[shift + 7], "Dated"),
                Common.CheckParam(actualIncomingLetter.InNumber, parameters[shift + 8], "InNumber"),
                Common.CheckParam(actualIncomingLetter.Addressee, parameters[shift + 9], "Addressee"),
                Common.CheckParam(actualIncomingLetter.Note, parameters[shift + 10], "Note"),
                Common.CheckParam(actualIncomingLetter.DeliveryMethod, parameters[shift + 11], "DeliveryMethod"),
                Common.CheckParam(actualIncomingLetter.DocumentRegister, parameters[shift + 12], "DocumentRegister"),
                Common.CheckParam(actualIncomingLetter.RegistrationState, BusinessLogic.GetRegistrationsState(parameters[shift + 13].Trim()), "RegistrationState")         
            };

            errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (errorList.Any())
                errorList.Insert(0, $"Ошибка в сущности: {name}");

            return string.Join(Environment.NewLine, errorList);
        }
    }
}