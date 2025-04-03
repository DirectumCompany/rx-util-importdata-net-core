using System.Globalization;
using System.Security.Cryptography;
using DocumentFormat.OpenXml.VariantTypes;
using ImportData;
using ImportData.Entities.Databooks;
using ImportData.IntegrationServicesClient.Models;
using System.Linq;
using Xunit.Extensions.Ordering;
using Microsoft.VisualBasic;

namespace Tests.EDocs
{

    public partial class Tests
    {
        [Fact, Order(90)]
        public void T9_SimpleDocumentImport()
        {
            var xlsxPath = TestSettings.SimpleDocumentPathXlsx;
            var action = ImportData.Constants.Actions.ImportSimpleDocument;
            var sheetName = ImportData.Constants.SheetNames.SimpleDocument;

            var items = Common.XlsxParse(xlsxPath, sheetName);

            Program.Main(Common.GetArgs(action, xlsxPath));

            var errorList = new List<string>();
            foreach (var expectedSimpleDocument in items)
            {
                var error = EqualsSimpleDocument(expectedSimpleDocument);

                if (string.IsNullOrEmpty(error))
                    continue;

                errorList.Add(error);
            }
            if (errorList.Any())
                Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, errorList));
        }

        public static string EqualsSimpleDocument(List<string> parameters, int shift = 0)
        {
            var exceptionList = new List<Structures.ExceptionsStruct>();
            var docDate = IEntityBase.GetDate(parameters[shift + 1].Trim());
            var docRegisterId = parameters[shift].Trim();

            var actualSimpleDocument = BusinessLogic.GetEntityWithFilter<ISimpleDocument>(x => x.RegistrationNumber != null &&
                x.RegistrationNumber == docRegisterId &&
                x.DocumentDate == docDate, exceptionList, TestSettings.Logger, true);
            // var actualSimpleDocument = Common.GetOfficialDocument<ISimpleDocument>(parameters[shift + 0], parameters[shift + 1], parameters[shift + 8]);
            var name = Common.GetDocumentName(parameters[shift + 3], parameters[shift + 0], parameters[shift + 1], parameters[shift + 4]);

            if (actualSimpleDocument == null)
                return $"Не найден простой документ: {name}";

            var errorList = new List<string>
            {
                Common.CheckParam(actualSimpleDocument.RegistrationNumber, parameters[shift + 0], "RegistrationNumber"),
                Common.CheckParam(actualSimpleDocument.RegistrationDate, parameters[shift + 1], "RegistrationDate"),
                Common.CheckParam(actualSimpleDocument.DocumentKind?.Name, parameters[shift + 3], "DocumentKind"),
                Common.CheckParam(actualSimpleDocument.Subject, parameters[shift + 4], "Subject"),
                Common.CheckParam(actualSimpleDocument.LastVersion(), parameters[shift + 5], "LastVersion"),
                Common.CheckParam(actualSimpleDocument.LifeCycleState, BusinessLogic.GetPropertyLifeCycleState(parameters[shift + 6]), "LifeCycleState"),
                Common.CheckParam(actualSimpleDocument.Note, parameters[shift + 7], "Note"),
                Common.CheckParam(actualSimpleDocument.DocumentRegister?.Id, parameters[shift + 8], "DocumentRegister"),
                Common.CheckParam(actualSimpleDocument.RegistrationState, BusinessLogic.GetRegistrationsState(parameters[shift + 9]), "RegistrationState"),
                Common.CheckParam(actualSimpleDocument.CaseFile?.Name, parameters[shift + 10], "CaseFile"),
                Common.CheckParam(actualSimpleDocument.PlacedToCaseFileDate?.UtcDateTime, parameters[shift + 11], "PlacedToCaseFileDate")
            };

            errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (errorList.Any())
                errorList.Insert(0, $"Ошибка в сущности: {name}");

            return string.Join(Environment.NewLine, errorList);
        }
    }
}
