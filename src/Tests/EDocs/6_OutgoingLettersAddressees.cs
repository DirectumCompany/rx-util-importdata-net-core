using DocumentFormat.OpenXml.Wordprocessing;
using ImportData;
using ImportData.Entities.Databooks;
using ImportData.IntegrationServicesClient.Models;
using Xunit.Extensions.Ordering;

namespace Tests.EDocs
{
    public partial class Tests
    {
        [Fact, Order(60)]
        public void T6_OutgoingLettersAddresseesImport()
        {
            Assert.Fail("Табличная часть тестируется руками)");

            var xlsxPath = TestSettings.OutgoingLettersAddresseesPathXlsx;
            var action = ImportData.Constants.Actions.ImportOutgoingLettersAddressees;
            var sheetName = ImportData.Constants.SheetNames.OutgoingLettersAddressees;

            var items = Common.XlsxParse(xlsxPath, sheetName);

            Program.Main(Common.GetArgs(action, xlsxPath));

            var errorList = new List<string>();
            foreach (var expectedOutgoingLettersAddressees in items)
            {
                var error = EqualsOutgoingLettersAddressees(expectedOutgoingLettersAddressees);

                if (string.IsNullOrEmpty(error))
                    continue;

                errorList.Add(error);
            }
            if (errorList.Any())
                Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, errorList));
        }

        public static string EqualsOutgoingLettersAddressees(List<string> parameters, int shift = 0)
        {
            var exceptionList = new List<Structures.ExceptionsStruct>();

            var outgoingDocumentBaseId = int.Parse(parameters[shift + 0].Trim());
            var actualOutgoingLetters = BusinessLogic.GetEntityWithFilter<IOutgoingLetters>(x => true, exceptionList, TestSettings.Logger, true);

            var correspondent = parameters[shift + 1].Trim();
            var addressee = parameters[shift + 2].Trim();
            var deliveryMethod = parameters[shift + 3].Trim();
            var actualOutgoingLettersAddressees = BusinessLogic.GetEntityWithFilter<IOutgoingLetterAddresseess>(x => x.OutgoingDocumentBase.Id == outgoingDocumentBaseId &&
            x.Correspondent.Name == correspondent &&
            x.Addressee.Name == addressee &&
            x.DeliveryMethod.Name == deliveryMethod, exceptionList, TestSettings.Logger, true);

            if (actualOutgoingLettersAddressees == null)
                return $"Не найдено входящее письмо с id: {parameters[shift + 0]}";

            var errorList = new List<string>
            {
                Common.CheckParam(actualOutgoingLettersAddressees.OutgoingDocumentBase, parameters[shift + 0], "OutgoingDocumentBase"),
                Common.CheckParam(actualOutgoingLettersAddressees.Correspondent, parameters[shift + 1], "Correspondent"),
                Common.CheckParam(actualOutgoingLettersAddressees.Addressee, parameters[shift + 2], "Addressee"),
                Common.CheckParam(actualOutgoingLettersAddressees.DeliveryMethod, parameters[shift + 3], "DeliveryMethod"),
            };

            errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (errorList.Any())
                errorList.Insert(0, $"Ошибка в сущности:");

            return string.Join(Environment.NewLine, errorList);
        }
    }
}