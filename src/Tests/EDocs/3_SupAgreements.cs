using ImportData;
using Xunit.Extensions.Ordering;

namespace Tests.EDocs
{
    public partial class Tests
    {
        [Fact, Order(30)]
        public void T3_SupAgreementsImport()
        {
            var xlsxPath = TestSettings.SupagreementsPathXlsx;
            var action = ImportData.Constants.Actions.ImportSupAgreements;
            var sheetName = ImportData.Constants.SheetNames.SupAgreements;

            var items = Common.XlsxParse(xlsxPath, sheetName);

            Program.Main(Common.GetArgs(action, xlsxPath));

            var errorList = new List<string>();
            foreach (var expectedSupAgreement in items)
            {
                var error = EqualsSupAgreement(expectedSupAgreement);

                if (string.IsNullOrEmpty(error))
                    continue;

                errorList.Add(error);
            }
            if (errorList.Any())
                Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, errorList));
        }

        public static string EqualsSupAgreement(List<string> parameters, int shift = 0)
        {
            var actualSupAgreement = Common.GetOfficialDocument<ISupAgreements>(parameters[shift + 0], parameters[shift + 1]);
            var counterpartyName = parameters[shift + 4];
            var counterparty = BusinessLogic.GetEntityWithFilter<ICounterparties>(c => c.Name == counterpartyName, new List<Structures.ExceptionsStruct>(), TestSettings.Logger);
            var counterpartyId = counterparty?.Id ?? -1;
			var leadDocSearchResult = IOfficialDocuments.GetLeadingDocument(TestSettings.Logger, parameters[shift + 2], Common.ParseDate(parameters[shift + 3]), counterpartyId);
            var name = Common.GetDocumentName(parameters[shift + 5], parameters[shift + 0], parameters[shift + 1], parameters[shift + 6]);

            if (!string.IsNullOrEmpty(leadDocSearchResult.errorMessage))
                return leadDocSearchResult.errorMessage;

            if (actualSupAgreement == null)
                return $"Не найдено дополнительное соглашение: {name}";

            var leadingDocument = leadDocSearchResult.leadingDocument;

            var errorList = new List<string>
            {
                Common.CheckParam(actualSupAgreement.RegistrationNumber, parameters[shift + 0], "RegistrationNumber"),
                Common.CheckParam(actualSupAgreement.RegistrationDate, parameters[shift + 1], "RegistrationDate"),
                Common.CheckParam(leadingDocument.RegistrationNumber, parameters[shift + 2], "LeadingDocumentRegNumber"),
                Common.CheckParam(leadingDocument.RegistrationDate, parameters[shift + 3], "LeadingDocumentRegNumberRegDate"),
                Common.CheckParam(actualSupAgreement.Counterparty, parameters[shift + 4], "Counterparty"),
                Common.CheckParam(actualSupAgreement.DocumentKind , parameters[shift + 5], "DocumentKind"),
                Common.CheckParam(actualSupAgreement.Subject, parameters[shift + 6], "Subject"),
                Common.CheckParam(actualSupAgreement.BusinessUnit, parameters[shift + 7], "BusinessUnit"),
                Common.CheckParam(actualSupAgreement.Department, parameters[shift + 8], "Department"),
                Common.CheckParam(actualSupAgreement.LastVersion(), parameters[shift + 9], "LastVersion"),
                Common.CheckParam(actualSupAgreement.ValidFrom, parameters[shift + 10], "ValidFrom"),
                Common.CheckParam(actualSupAgreement.ValidTill, parameters[shift + 11], "ValidTill"),
                Common.CheckParam(actualSupAgreement.TotalAmount, parameters[shift + 12], "TotalAmount"),
                Common.CheckParam(actualSupAgreement.Currency, parameters[shift + 13], "Currency"),
                Common.CheckParam(actualSupAgreement.LifeCycleState, BusinessLogic.GetPropertyLifeCycleState(parameters[shift + 14]), "LifeCycleState"),
                Common.CheckParam(actualSupAgreement.ResponsibleEmployee , parameters[shift + 15], "ResponsibleEmployee"),
                Common.CheckParam(actualSupAgreement.OurSignatory, parameters[shift + 16], "OurSignatory"),
                Common.CheckParam(actualSupAgreement.Note, parameters[shift + 17], "Note"),
                Common.CheckParam(actualSupAgreement.DocumentRegister?.Id, parameters[shift + 18], "DocumentRegister"),
                Common.CheckParam(actualSupAgreement.RegistrationState, BusinessLogic.GetRegistrationsState(parameters[shift + 19]), "RegistrationState"),
                Common.CheckParam(actualSupAgreement.CaseFile, parameters[shift + 20], "CaseFile"),
                Common.CheckParam(actualSupAgreement.PlacedToCaseFileDate, parameters[shift + 21], "PlacedToCaseFileDate")
            };

            errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (errorList.Any())
                errorList.Insert(0, $"Ошибка в сущности: {name}");

            return string.Join(Environment.NewLine, errorList);
        }
    }
}