using System.Globalization;
using System.Security.Cryptography;
using DocumentFormat.OpenXml.VariantTypes;
using ImportData;
using ImportData.Entities.Databooks;
using ImportData.IntegrationServicesClient.Models;
using System.Linq;
using Xunit.Extensions.Ordering;

namespace Tests.EDocs
{
    [Order(20)]
    public partial class Tests
    {
        [Fact, Order(10)]
        public void T1_ContractsImport()
        {
            var xlsxPath = TestSettings.ContractsPathXlsx;
            var action = ImportData.Constants.Actions.ImportContracts;
            var sheetName = ImportData.Constants.SheetNames.Contracts;

            var items = Common.XlsxParse(xlsxPath, sheetName);

            // �������� ��������� �������� �� �����.
            foreach (var item in items)
            {
                CreateDocumentGroup(item);
            }

            Program.Main(Common.GetArgs(action, xlsxPath));

            var errorList = new List<string>();
            foreach (var expectedContract in items)
            {
                var error = EqualsContract(expectedContract);

                if (string.IsNullOrEmpty(error))
                    continue;

                errorList.Add(error);
            }
            if (errorList.Any())
                Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, errorList));
        }

        public static string EqualsContract(List<string> parameters, int shift = 0)
        {
            var actualContract = Common.GetOfficialDocument<IContracts>(parameters[shift + 0], parameters[shift + 1], parameters[shift + 17]);
            var name = Common.GetDocumentName(parameters[shift + 3], parameters[shift + 0], parameters[shift + 1], parameters[shift + 5]);
        
            if (actualContract == null)
                return $"�� ������ �������: {name}";

            var errorList = new List<string>
            {
                Common.CheckParam(actualContract.RegistrationNumber, parameters[shift + 0], "RegistrationNumber"),
                Common.CheckParam(actualContract.RegistrationDate, parameters[shift + 1], "RegistrationDate"),
                Common.CheckParam(actualContract.Counterparty, parameters[shift + 2], "Counterparty"),
                Common.CheckParam(actualContract.DocumentKind, parameters[shift + 3], "DocumentKind"),
                Common.CheckParam(actualContract.DocumentGroup, parameters[shift + 4], "DocumentGroup"),
                Common.CheckParam(actualContract.Subject, parameters[shift + 5], "Subject"),
                Common.CheckParam(actualContract.BusinessUnit, parameters[shift + 6], "BusinessUnit"),
                Common.CheckParam(actualContract.Department, parameters[shift + 7], "Department"),
                Common.CheckParam(actualContract.LastVersion(), parameters[shift + 8], "LastVersion"),
                Common.CheckParam(actualContract.ValidFrom, parameters[shift + 9], "ValidFrom"),
                Common.CheckParam(actualContract.ValidTill, parameters[shift + 10], "ValidTill"),
                Common.CheckParam(actualContract.TotalAmount, parameters[shift + 11], "TotalAmount"),
                Common.CheckParam(actualContract.Currency, parameters[shift + 12], "Currency"),
                Common.CheckParam(actualContract.LifeCycleState, BusinessLogic.GetPropertyLifeCycleState(parameters[shift + 13]), "LifeCycleState"),
                Common.CheckParam(actualContract.ResponsibleEmployee, parameters[shift + 14], "ResponsibleEmployee"),
                Common.CheckParam(actualContract.OurSignatory, parameters[shift + 15], "OurSignatory"),
                Common.CheckParam(actualContract.Note, parameters[shift + 16], "Note"),
                Common.CheckParam(actualContract.DocumentRegister?.Id, parameters[shift + 17], "DocumentRegister"),
                Common.CheckParam(actualContract.RegistrationState, BusinessLogic.GetRegistrationsState(parameters[shift + 18]), "RegistrationState")
            };

            errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (errorList.Any())
                errorList.Insert(0, $"������ � ��������: {name}");

            return string.Join(Environment.NewLine, errorList);
        }

        public static IContractCategories CreateDocumentGroup(List<string> parameters, int shift = 0)
        {
            Common.InitODataClient();

            var exceptionList = new List<Structures.ExceptionsStruct>();
            var name = parameters[shift + 4].Trim();
            var contractCategory = BusinessLogic.GetEntityWithFilter<IContractCategories>(c => c.Name == name, exceptionList, TestSettings.Logger, true);

            if (contractCategory == null)
            {
                contractCategory = new IContractCategories() { Name = name, Status = "Active" };
                contractCategory = BusinessLogic.CreateEntity(contractCategory, exceptionList, TestSettings.Logger);
            }

            return contractCategory;
        }
    }
}
