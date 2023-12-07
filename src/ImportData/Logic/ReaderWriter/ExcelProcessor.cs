using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Threading;
using NLog;

namespace ImportData.Logic.ReaderWriter
{
    public class ExcelProcessor
    {
        public string DocPath { get; set; }
        public string WorksheetName { get; set; }
        private static Logger logger;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public ExcelProcessor(string docPath, string worksheetName, Logger loggerParrent)
        {
            DocPath = docPath;
            WorksheetName = worksheetName;
            logger = loggerParrent;
        }

        /// <summary>
        /// Получить наименование колонки по порядковому номеру.
        /// </summary>
        /// <param name="columnNumber">Порядковый номер.</param>
        /// <returns>Наименование колонки.</returns>
        public string GetExcelColumnName(int columnNumber)
        {
            if (columnNumber <= 26)
            {
                return Convert.ToChar(columnNumber + 64).ToString();
            }
            int div = columnNumber / 26;
            int mod = columnNumber % 26;
            if (mod == 0)
            { mod = 26; div--; }
            return GetExcelColumnName(div) + GetExcelColumnName(mod);
        }

        /// <summary>
        /// Получить рабочий лист по имени.
        /// </summary>
        /// <param name="document">Экземпляр типа SpreadsheetDocument.</param>
        /// <param name="sheetName">Наименование листа.</param>
        /// <returns>Рабочий лист.</returns>
        private static WorksheetPart GetWorksheetPartByName(SpreadsheetDocument document, string sheetName)
        {
            IEnumerable<Sheet> sheets =
               document.WorkbookPart.Workbook.GetFirstChild<Sheets>().
               Elements<Sheet>().Where(s => s.Name == sheetName);
            if (sheets?.Count() == 0)
            {
                logger.Error(string.Format("В XLSX-файле не найден лист с именем \"{0}\".", sheetName));
                return null;
            }
            string relationshipId = sheets?.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(relationshipId);
            return worksheetPart;
        }

        /// <summary>
        /// Запись строк в ячейки.
        /// </summary>
        /// <param name="listImportData">Список импортируемых записей.</param>
        public void InsertText(List<ArrayList> listImportData, int parametersListCount)
        {
            uint row = 2;
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(DocPath, true))
            {
                SharedStringTablePart shareStringPart;
                if (doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                {
                    shareStringPart = doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                }
                else
                {
                    shareStringPart = doc.WorkbookPart.AddNewPart<SharedStringTablePart>();
                }

                WorkbookPart workbookPart = doc.WorkbookPart;
                SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                SharedStringTable sst = sstpart.SharedStringTable;
                WorksheetPart worksheetPart = GetWorksheetPartByName(doc, WorksheetName);
                Worksheet sheet = worksheetPart.Worksheet;

                foreach (var importItems in listImportData)
                {
                    int index = InsertSharedStringItem(Convert.ToString(importItems[0]), shareStringPart);

                    Cell cell = InsertCellInWorksheet(Convert.ToString(importItems[1]), Convert.ToUInt32(importItems[2]), worksheetPart);

                    cell.CellValue = new CellValue(index.ToString());
                    cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                    if (Convert.ToInt32(importItems[2]) != 1 && (row - 1) % 3 == 0)
                    {
                        var percent = (row - 2) / 3 / (double)parametersListCount * 100.00;
                        logger.Info($"\rОбработано {(row - 2) / 3} строк из {parametersListCount} ({percent:F2}%)");
                    }
                    row++;
                }
                worksheetPart.Worksheet.Save();
            }
        }

        /// <summary>
        /// Учитывая имя столбца, индекс строки и WorksheetPart, вставляет ячейку в лист. Если ячейка уже существует, возвращает ее.
        /// </summary>
        /// <param name="columnName">Наименование столбца.</param>
        /// <param name="rowIndex">Номер строки.</param>
        /// <param name="worksheetPart">Рабочая область.</param>
        /// <returns>Ячейка.</returns>
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            if (row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (cell.CellReference.Value.Length == cellReference.Length)
                    {
                        if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }

        /// <summary>
        /// Данный текст и SharedStringTablePart создает SharedStringItem с указанным текстом и вставляет его в SharedStringTablePart. Если элемент уже существует, возвращает его индекс.
        /// </summary>
        /// <param name="text">Импортируемый текст.</param>
        /// <param name="shareStringPart">Формат строки.</param>
        /// <returns>Индекс элемента.</returns>
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }
                i++;
            }

            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        /// <summary>
        /// Получить структуры данных из файла Excel.
        /// </summary>
        /// <returns>Матрица полей.</returns>
        public IEnumerable<List<string>> GetDataFromExcel()
        {
            var result = new List<List<string>>();
            try
            {
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(DocPath, false))
                {
                    WorkbookPart workbookPart = doc.WorkbookPart;
                    SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                    SharedStringTable sst = sstpart.SharedStringTable;
                    WorksheetPart worksheetPart = null;
                    try
                    {
                        worksheetPart = GetWorksheetPartByName(doc, WorksheetName);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Не удалось обработать файл xlsx. Не найдена страница с наименованием \"{0}\". Подробности: {1}", WorksheetName, ex.Message), ex);
                    }
                    Worksheet sheet = worksheetPart.Worksheet;
                    var cells = sheet.Descendants<Cell>();
                    var rows = sheet.Descendants<Row>();

                    var maxElements = rows.First().Elements<Cell>().Count();
                    var rowNum = 0;
                    foreach (Row row in rows)
                    {
                        rowNum++;
                        // HACK OpenText пропускает пустые ячейки. Будем импровизировать. С помощью функции CellReferenceToIndex будем определять "правильный" порядковый номер ячейки и заполнять массив.
                        var rowData = new string[maxElements];
                        for (var i = 0; i < maxElements; i++)
                            rowData[i] = string.Empty;

                        foreach (Cell c in row.Elements<Cell>())
                        {
                            if (c.CellValue != null)
                            {
                                var innerText = c.DataType != null && c.DataType == CellValues.SharedString ?
                                  sst.ChildElements[int.Parse(c.CellValue.Text)].InnerText :
                                  c.CellValue.InnerText;
                                rowData[CellReferenceToIndex(c)] = innerText;
                            }
                        }
                        // Если максимальное значение в наборе данных - пустое значение, значит строка не содержит данных. 
                        if (!string.IsNullOrWhiteSpace(rowData.Max()))
                            result.Add(rowData.ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Не удалось обработать файл xlsx. Обратитесь к администратору системы. Подробности: {0}", ex.Message), ex);
            }
            return result;
        }

        /// <summary>
        /// Поиск правильного индекса ячейки.
        /// </summary>
        /// <param name="cell">Ячейка.</param>
        /// <returns>Индекс.</returns>
        private static int CellReferenceToIndex(Cell cell)
        {
            int index = 0;
            int countChar = 0;
            string reference = cell.CellReference.ToString().ToUpper();
            foreach (char ch in reference)
            {
                countChar++;
                if (char.IsLetter(ch))
                {
                    int value = ch - 'A';
                    //index = (index == 0) ? value : ((index + 1) * 26) + value;
                    if (index == 0)
                        index = value;
                    if (countChar > 1)
                        index = 26 + index;
                }
                else
                    return index;
            }
            return index;
        }

    }
}
