using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Security.Cryptography;
using ImportData;
using ImportData.IntegrationServicesClient.Models;
using Xunit.Extensions.Ordering;
using DocumentFormat.OpenXml.Office2010.Word;

namespace Tests.EDocs
{
    public partial class Tests
    {
        [Fact, Order(70)]
        public void T7_OrdersImport()
        {
            var xlsxPath = TestSettings.OrdersPathXlsx;
            var action = ImportData.Constants.Actions.ImportOrders;
            var sheetName = ImportData.Constants.SheetNames.Orders;

            var items = Common.XlsxParse(xlsxPath, sheetName);

            Program.Main(Common.GetArgs(action, xlsxPath));

            var errorList = new List<string>();
            foreach (var expectedOrder in items)
            {
                var error = EqualsOrders(expectedOrder);

                if (string.IsNullOrEmpty(error))
                    continue;

                errorList.Add(error);
            }
            if (errorList.Any())
                Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, errorList));
        }

        public static string EqualsOrders(List<string> parameters, int shift = 0)
        {
            var exceptionList = new List<Structures.ExceptionsStruct>();

            var actualOrder = Common.GetOfficialDocument<IOrders>(parameters[shift + 0], parameters[shift + 1], parameters[shift + 12]);

            var name = Common.GetDocumentName(parameters[shift + 2], parameters[shift + 0], parameters[shift + 1], parameters[shift + 3]);

            if (actualOrder == null)
                return $"Не найден приказ: {name}";

            var errorList = new List<string>
            {
                Common.CheckParam(actualOrder.RegistrationNumber, parameters[shift + 0], "RegistrationNumber"),
                Common.CheckParam(actualOrder.RegistrationDate, parameters[shift + 1], "RegistrationDate"),
                Common.CheckParam(actualOrder.DocumentKind, parameters[shift + 2], "DocumentKind"),
                Common.CheckParam(actualOrder.Subject, parameters[shift + 3], "Subject"),
                Common.CheckParam(actualOrder.BusinessUnit, parameters[shift + 4], "BusinessUnit"),
                Common.CheckParam(actualOrder.Department, parameters[shift + 5], "Department"),
                Common.CheckParam(actualOrder.LastVersion(), parameters[shift + 6], "LastVersion"),
                Common.CheckParam(actualOrder.Assignee, parameters[shift + 7], "Assignee"),
                Common.CheckParam(actualOrder.PreparedBy, parameters[shift + 8], "PreparedBy"),
                Common.CheckParam(actualOrder.OurSignatory, parameters[shift + 9], "OurSignatory"),
                Common.CheckParam(actualOrder.LifeCycleState, BusinessLogic.GetPropertyLifeCycleState(parameters[shift + 10]), "LifeCycleState"),
                Common.CheckParam(actualOrder.Note, parameters[shift + 11], "Note"),
                Common.CheckParam(actualOrder.DocumentRegister, parameters[shift + 12], "DocumentRegister"),
                Common.CheckParam(actualOrder.RegistrationState, BusinessLogic.GetRegistrationsState(parameters[shift + 13]), "RegistrationState")
            };

            errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (errorList.Any())
                errorList.Insert(0, $"Ошибка в сущности: {name}");

            return string.Join(Environment.NewLine, errorList);
        }
    }
}