using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ImportData.Logic.ReaderWriter
{
	internal class CsvReaderWriter : IFileReaderWriter
	{
		private Logger Logger { get; set; }
		private string FilePath { get; set; }
		private CsvConfiguration Config { get; set; }

		public CsvReaderWriter(Logger logger, string filePath, string delimiter)
		{
			Logger = logger;
			FilePath = filePath;
			Config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				Delimiter = delimiter
			};
		}

		public List<string[]> Read()
		{
			Logger.Info("===================Чтение строк из файла===================");
			var watch = Stopwatch.StartNew();
			var result = new List<string[]>();
			var row = new List<string>();
			using var reader = new StreamReader(FilePath);
			using var csvReader = new CsvReader(reader, Config);
			while (csvReader.Read())
			{
				for (int i = 0; csvReader.TryGetField<string>(i, out string value); i++)
				{
					row.Add(value);
				}
				result.Add(row.ToArray());
				row.Clear();
			}
			watch.Stop();
			var elapsedMs = watch.ElapsedMilliseconds;
			Logger.Info($"Времени затрачено на чтение строк из файла: {elapsedMs} мс");
			return result;
		}

		public void Write(List<List<Structures.ExceptionsStruct>> exceptions, int paramCount)
		{
			Logger.Info("=============Запись результатов импорта в файл=============");
			var watch = Stopwatch.StartNew();
			var oldRows = Read();
			var needUpdateLastRows = false;
			using var writer = new StreamWriter(FilePath);
			using var csvWriter = new CsvWriter(writer, Config);
			for(var i = 0; i < oldRows.Count; i++)
			{
				var oldRow = oldRows[i];
				IEnumerable<string> newRow = null;
				if (i == 0)
				{
					if (!oldRow.Contains("Итог"))
					{
						var title = new string[] { "Итог", "Дата", "Подробности" };
						newRow = oldRow.Concat(title);
					}
					else
					{
						newRow = oldRow;
						needUpdateLastRows = true;
					}
				}
				else
				{
					if (needUpdateLastRows)
						oldRow = oldRow.Take(oldRow.Count() - 3).ToArray();
					newRow = GetNewRow(oldRow, exceptions[i - 1]);
				}

				csvWriter.WriteField(newRow.ToArray());
				csvWriter.NextRecord();
			}

			watch.Stop();
			var elapsedMs = watch.ElapsedMilliseconds;
			Logger.Info($"Времени затрачено на запись результатов в файл: {elapsedMs} мс");
		}

		private IEnumerable<string> GetNewRow(IEnumerable<string> oldRow, IEnumerable<Structures.ExceptionsStruct> exceptionList)
		{
			string[] exceptionResult;
			if (exceptionList.Where(x => x.ErrorType == Constants.ErrorTypes.Error).Any())
			{
				var message = string.Join("; ", exceptionList.Where(x => x.ErrorType == Constants.ErrorTypes.Error).Select(x => x.Message).ToArray());
				exceptionResult = new string[] { "Не загружен", DateTime.Now.ToString("d"), message };
			}
			else if (exceptionList.Where(x => x.ErrorType == Constants.ErrorTypes.Warn).Any())
			{
				var message = string.Join(Environment.NewLine, exceptionList.Where(x => x.ErrorType == Constants.ErrorTypes.Warn).Select(x => x.Message).ToArray());
				exceptionResult = new string[] { "Загружен частично", DateTime.Now.ToString("d"), message };
			}
			else
			{
				exceptionResult = new string[] { "Загружен", DateTime.Now.ToString("d"), string.Empty };
			}

			return oldRow.Concat(exceptionResult);
		}
	}
}
