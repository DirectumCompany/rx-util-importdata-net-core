using ImportData;

namespace Tests.EDocs
{
    public partial class Tests
    {
        [Fact]
        public void OutgoingLettersImport()
        {
            var xlsxPath = TestSettings.OutgoingLettersPathXlsx;
            var action = ImportData.Constants.Actions.ImportOutgoingLetters;
            var sheetName = ImportData.Constants.SheetNames.OutgoingLetters;
            var logger = TestSettings.Logger;
            var items = Common.XlsxParse(xlsxPath, sheetName, logger);

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
            var exceptionList = new List<Structures.ExceptionsStruct>();
            var regNumber = parameters[shift + 0].Trim();

            DateTimeOffset regDate = DateTimeOffset.MinValue;

            var regDateBeginningOfDay = Common.ParseDate(parameters[shift + 1].Trim());
            var counterpartyName = parameters[shift + 2].Trim();
            var documentRegisterId = -1;
            int.TryParse(parameters[shift + 17].Trim(), out documentRegisterId);


            var actualOutgoingLetter = BusinessLogic.GetEntityWithFilter<IOutgoingLetters>(x => x.RegistrationNumber != null &&
                                                                                    x.RegistrationNumber == regNumber &&
                                                                                    x.RegistrationDate == regDateBeginningOfDay &&
                                                                                    x.DocumentRegister.Id == documentRegisterId, exceptionList, TestSettings.Logger, true);

            if (actualOutgoingLetter == null)
                return $"�� ������� �������������� ����������";

            var errorList = new List<string>
            {

            };

            errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (errorList.Any())
                errorList.Insert(0, $"������ � ��������:");

            return string.Join(Environment.NewLine, errorList);
        }
    }
}