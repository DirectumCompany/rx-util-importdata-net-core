using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;
using NLog;

namespace ImportData.Logic.ReaderWriter
{
  internal class ExcelReaderWriter : IFileReaderWriter
  {
		private Logger logger { get; set; }
    private ExcelProcessor excelProcessor { get; set; }

		public ExcelReaderWriter(Logger logger, string filePath, string sheetName) 
    {
      this.logger = logger;
			this.excelProcessor = new ExcelProcessor(filePath, sheetName, logger);
    }
    public List<string[]> Read()
    {
			var importData = excelProcessor.GetDataFromExcel();

			logger.Info("===================Чтение строк из файла===================");
			var watch = Stopwatch.StartNew();
			var arrayItems = new ArrayList();
			uint row = 2;
			var parametersListCount = importData.Count();
			var listImportItems = new List<string[]>();
			foreach (var importItem in importData)
			{
				int countItem = importItem.Count();
				foreach (var data in importItem.Take(countItem - 3))
					arrayItems.Add(data);

				listImportItems.Add((string[])arrayItems.ToArray(typeof(string)));
				var percent = (double)(row - 1) / (double)parametersListCount * 100.00;
				logger.Info($"\rОбработано {row - 1} строк из {parametersListCount} ({percent:F2}%)");
				arrayItems.Clear();
				row++;
			}
			watch.Stop();
			var elapsedMs = watch.ElapsedMilliseconds;
			logger.Info($"Времени затрачено на чтение строк из файла: {elapsedMs} мс");


			return listImportItems;

		}

    public void Write(List<List<Structures.ExceptionsStruct>> listResult, int paramCount)
    {
			logger.Info("=============Запись результатов импорта в файл=============");
			var watch = Stopwatch.StartNew();
			var parametersListCount = listResult.Count;
			uint row = 2;

			var listArrayParams = new List<ArrayList>();
			string[] text = new string[] { "Итог", "Дата", "Подробности" };
			for (int i = 1; i <= 3; i++)
			{
				var title = excelProcessor.GetExcelColumnName(paramCount + i);
				var arrayParams = new ArrayList { text[i - 1], title, 1 };
				listArrayParams.Add(arrayParams);
			}

			foreach (var result in listResult)
			{
				if (result.Where(x => x.ErrorType == Constants.ErrorTypes.Error).Any())
				{
					// TODO: Добавить локализацию строки.
					var message = string.Join("; ", result.Where(x => x.ErrorType == Constants.ErrorTypes.Error).Select(x => x.Message).ToArray());
					text = null;
					text = new string[] { "Не загружен", DateTime.Now.ToString("d"), message };
					for (int i = 1; i <= 3; i++)
					{
						var title = excelProcessor.GetExcelColumnName(paramCount + i);
						var arrayParams = new ArrayList { text[i - 1], title, row };
						listArrayParams.Add(arrayParams);
					}
				}
				else if (result.Where(x => x.ErrorType == Constants.ErrorTypes.Warn).Any())
				{
					// TODO: Добавить локализацию строки.
					var message = string.Join(Environment.NewLine, result.Where(x => x.ErrorType == Constants.ErrorTypes.Warn).Select(x => x.Message).ToArray());
					text = null;
					text = new string[] { "Загружен частично", DateTime.Now.ToString("d"), message };
					for (int i = 1; i <= 3; i++)
					{
						var title = excelProcessor.GetExcelColumnName(paramCount + i);
						var arrayParams = new ArrayList { text[i - 1], title, row };
						listArrayParams.Add(arrayParams);
					}
				}
				else
				{
					// TODO: Добавить локализацию строки.
					text = null;
					text = new string[] { "Загружен", DateTime.Now.ToString("d"), string.Empty };
					for (int i = 1; i <= 3; i++)
					{
						var title = excelProcessor.GetExcelColumnName(paramCount + i);
						var arrayParams = new ArrayList { text[i - 1], title, row };
						listArrayParams.Add(arrayParams);
					}
				}
				row++;
			}
			excelProcessor.InsertText(listArrayParams, parametersListCount);
			watch.Stop();
			var elapsedMs = watch.ElapsedMilliseconds;
			logger.Info($"Времени затрачено на запись результатов в файл: {elapsedMs} мс");
		}
  }
}
