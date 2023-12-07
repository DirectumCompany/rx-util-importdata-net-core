using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using ImportData;
using ImportData.Entities.Databooks;

namespace Tests
{
    internal static class Common
    {
        /// <summary>
        /// Получить строку выполнения утилиты.
        /// </summary>
        /// <param name="action">Действие.</param>
        /// <param name="xlsxPath">Путь к xlsx файлу.</param>
        /// <returns>Массив параметров.</returns>
        public static string[] GetArgs(string action, string xlsxPath) => new[] { "-n", TestSettings.Login, "-p", TestSettings.Password, "-a", action, "-f", xlsxPath };

        /// <summary>
        /// Инициализация OData клиента.
        /// </summary>
        public static void InitODataClient() => Program.Main(GetArgs(Constants.Actions.InitForTests, Constants.Actions.InitForTests));

        /// <summary>
        /// Парсинг xlsx файла.
        /// </summary>
        /// <param name="xlsxPath">Путь к файлу.</param>
        /// <param name="sheetName">Имя листа.</param>
        /// <param name="logger">Логгер.</param>
        /// <returns>Распарсенные строки.</returns>
        public static IEnumerable<List<string>> XlsxParse(string xlsxPath, string sheetName)
        {
            var logger = TestSettings.Logger;
            var excelProcessor = new ImportData.Logic.ReaderWriter.ExcelProcessor(xlsxPath, sheetName, logger);
            var items = excelProcessor.GetDataFromExcel();
            return items.Skip(1);
        }

        /// <summary>
        /// Преобразовать зачение в дату.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <returns>Преобразованная дата.</returns>
        /// <exception cref="FormatException" />
        public static DateTimeOffset ParseDate(string? value)
        {
            var style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            var culture = CultureInfo.CreateSpecificCulture("en-GB");

            if (!string.IsNullOrEmpty(value))
            {
                DateTimeOffset date;
                if (DateTimeOffset.TryParse(value.Trim(), culture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out date))
                    return date;

                var dateDouble = 0.0;
                if (double.TryParse(value.Trim(), style, culture, out dateDouble))
                    return new DateTimeOffset(DateTime.FromOADate(dateDouble), TimeSpan.Zero);

                throw new FormatException("Неверный формат строки.");
            }
            else
                return DateTimeOffset.MinValue;
        }

        /// <summary>
        /// Получить официальный документ.
        /// </summary>
        /// <param name="regNumber">Номер регистрации.</param>
        /// <param name="regDate">Дата регистрации.</param>
        /// <param name="docRegisterId">Журнал регистрации.</param>
        /// <returns>Официальный документ.</returns>
        public static T GetOfficialDocument<T>(string regNumber, string regDateStr, string docRegisterIdStr = "") where T : IOfficialDocuments
        {
            var exceptionList = new List<Structures.ExceptionsStruct>();
            var regDate = ParseDate(regDateStr);
            var checkDocRegister = string.IsNullOrWhiteSpace(docRegisterIdStr);
            if (!int.TryParse(docRegisterIdStr, out var docRegisterId))
                docRegisterId = -1;
            var document = BusinessLogic.GetEntityWithFilter<T>(x => x.RegistrationNumber != null &&
                                                                                    x.RegistrationNumber == regNumber &&
                                                                                    x.RegistrationDate.Value.ToString("d") == regDate.ToString("d") &&
                                                                                    (checkDocRegister || x.DocumentRegister.Id == docRegisterId),
                                                                                    exceptionList, TestSettings.Logger, true);
            return document;
        }

        /// <summary>
        /// Сформировать имя документа.
        /// </summary>
        /// <param name="docKind">Вид документа.</param>
        /// <param name="regNumber">Регистрационный номер.</param>
        /// <param name="regDate">Дата регистрации.</param>
        /// <param name="subject">Тема.</param>
        /// <returns>Имя документа.</returns>
        public static string GetDocumentName(string docKind, string regNumber, string regDate, string subject)
        {
            return $"{docKind} Номер {regNumber} от {regDate} \"{subject}\"";
        }

        #region Методы сравнения.
        /// <summary>
        /// Сравнить параметры и получить строку с ошибкой.
        /// </summary>
        /// <param name="actual">Актуальный (из системы).</param>
        /// <param name="expected">Ожидаемый (из xlsx).</param>
        /// <param name="paramName">Имя параметра.</param>
        /// <returns>Строку с ошибкой, если параметры не равны.</returns>
        public static string CheckParam(string? actual, string? expected, string paramName)
        {
            actual ??= string.Empty;
            expected ??= string.Empty;
            return actual == expected.Trim() ? string.Empty : $"ParamName: \"{paramName}\". Expected: \"{expected}\". Actual: \"{actual}\"";
        }

        /// <summary>
        /// Сравнить параметры и получить строку с ошибкой.
        /// </summary>
        /// <param name="actual">Актуальный (из системы).</param>
        /// <param name="expected">Ожидаемый (из xlsx).</param>
        /// <param name="paramName">Имя параметра.</param>
        /// <returns>Строку с ошибкой, если параметры не равны.</returns>
        public static string CheckParam(DateTimeOffset? actual, string expected, string paramName) => CheckParam((actual ?? DateTimeOffset.MinValue).ToString(), ParseDate(expected.Trim()).ToString(), paramName);

        /// <summary>
        /// Сравнить параметры и получить строку с ошибкой.
        /// </summary>
        /// <param name="actual">Актуальный (из системы).</param>
        /// <param name="expected">Ожидаемый (из xlsx).</param>
        /// <param name="paramName">Имя параметра.</param>
        /// <returns>Строку с ошибкой, если параметры не равны.</returns>
        public static string CheckParam(int? actual, string expected, string paramName) => CheckParam(actual.ToString(), expected.Trim(), paramName);

        /// <summary>
        /// Сравнить параметры и получить строку с ошибкой.
        /// </summary>
        /// <param name="actual">Актуальный (из системы).</param>
        /// <param name="expected">Ожидаемый (из xlsx).</param>
        /// <param name="paramName">Имя параметра.</param>
        /// <returns>Строку с ошибкой, если параметры не равны.</returns>
        public static string CheckParam(double? actual, string expected, string paramName) => CheckParam(actual.ToString(), expected.Trim(), paramName);

        /// <summary>
        /// Сравнить параметры и получить строку с ошибкой.
        /// </summary>
        /// <param name="actual">Актуальный (из системы).</param>
        /// <param name="expected">Ожидаемый (из xlsx).</param>
        /// <param name="paramName">Имя параметра.</param>
        /// <returns>Строку с ошибкой, если параметры не равны.</returns>
        public static string CheckParam(IEntity? actual, string expected, string paramName) => CheckParam(actual == null || string.IsNullOrEmpty(actual.Name) ? string.Empty : actual.Name, expected.Trim(), paramName);

        /// <summary>
        /// Сравнить параметры и получить строку с ошибкой.
        /// </summary>
        /// <param name="actual">Актуальный (из системы).</param>
        /// <param name="expected">Ожидаемый (из xlsx).</param>
        /// <param name="paramName">Имя параметра.</param>
        /// <returns>Строку с ошибкой, если параметры не равны.</returns>
        public static string CheckParam(ILogins? actual, string expected, string paramName) => CheckParam(actual == null ? string.Empty : actual.LoginName, expected.Trim(), paramName);

        /// <summary>
        /// Сравнить параметры и получить строку с ошибкой.
        /// </summary>
        /// <param name="actual">Актуальный (из системы).</param>
        /// <param name="expected">Ожидаемый (из xlsx).</param>
        /// <param name="paramName">Имя параметра.</param>
        /// <returns>Строку с ошибкой, если параметры не равны.</returns>
        public static string CheckParam(IDocumentRegisters? actual, string expected, string paramName) => CheckParam(actual == null ? -1 : actual.Id, expected.Trim(), paramName);

        /// <summary>
        /// Сравнить параметры и получить строку с ошибкой.
        /// </summary>
        /// <param name="actual">Актуальный (из системы).</param>
        /// <param name="expected">Ожидаемый (из xlsx).</param>
        /// <param name="paramName">Имя параметра.</param>
        /// <returns>Строку с ошибкой, если параметры не равны.</returns>
        public static string CheckParam(IElectronicDocumentVersionss? actual, string expected, string paramName)
        {
            expected = expected.Trim();

            if (actual == null && string.IsNullOrWhiteSpace(expected))
                return string.Empty;

            if (!File.Exists(expected))
                return $"ParamName: {paramName}. Файл не найден: {expected}";

            if (actual == null)
                return $"ParamName: {paramName}. Версия не загрузилась";

            return actual.Body.Value.SequenceEqual(File.ReadAllBytes(expected)) ? string.Empty : $"ParamName: {paramName}. Expected: {expected}. Бинарные данные различаются.";
        }
        #endregion
    }
}
