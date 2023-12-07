using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImportData;
using Xunit.Extensions.Ordering;

namespace Tests
{
    [Order(0)]
    public class Init
    {
        private static readonly Random newRegNumber = new();
        [Fact]
        public void T0_Init()
        {
            //Костыль. Чтоб RX не жаловался, что не может создать документ с уже существующим рег номером.
            var newNumbers = ChangeRegNumber(TestSettings.ContractsPathXlsx, Constants.SheetNames.Contracts, 1);
            ChangeRegNumber(TestSettings.AddendumsPathXlsx, Constants.SheetNames.Addendums, 1);
            ChangeRegNumber(TestSettings.AddendumsPathXlsx, Constants.SheetNames.Addendums, 3, newNumbers.ToList());
            ChangeRegNumber(TestSettings.SupagreementsPathXlsx, Constants.SheetNames.SupAgreements, 1);
            ChangeRegNumber(TestSettings.SupagreementsPathXlsx, Constants.SheetNames.SupAgreements, 3, newNumbers);
            ChangeRegNumber(TestSettings.IncomingLettersPathXlsx, Constants.SheetNames.IncomingLetters, 1);
            ChangeRegNumber(TestSettings.OutgoingLettersPathXlsx, Constants.SheetNames.OutgoingLetters, 1);
            ChangeRegNumber(TestSettings.OrdersPathXlsx, Constants.SheetNames.Orders, 1);
            ChangeRegNumber(TestSettings.CompanyDirectivesPathXlsx, Constants.SheetNames.CompanyDirectives, 1);
        }

        internal static List<ArrayList> ChangeRegNumber(string xlsxPath, string sheetName, int columnNumber, List<ArrayList>? newNumbers = null)
        {
            var logger = TestSettings.Logger;
            var excelProcessor = new ImportData.Logic.ReaderWriter.ExcelProcessor(xlsxPath, sheetName, logger);

            var items = Common.XlsxParse(xlsxPath, sheetName);
            var listArrayParams = new List<ArrayList>();
            var title = excelProcessor.GetExcelColumnName(columnNumber);
            for (var i = 0; i < items.Count(); i++)
            {
                var arrayParams = new ArrayList { newNumbers == null ? newRegNumber.Next(DateTime.Now.Millisecond) : newNumbers[i][0], title, i + 2 };
                listArrayParams.Add(arrayParams);
            }

            excelProcessor.InsertText(listArrayParams, listArrayParams.First().Count);

            return listArrayParams;
        }
    }
}
