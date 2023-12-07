using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using ImportData.Logic.ReaderWriter;
using ImportData.Logic;

namespace ImportData
{
  public class EntityProcessor
  {
    public static void Process(Type type, string filePath, string sheetName, Dictionary<string, string> extraParameters, Logger logger)
    {
      if (type.Equals(typeof(Entity)))
      {
        logger.Error(string.Format("Не найден соответствующий обработчик операции: {0}", "action"));
        return;
      }
      Type genericType = typeof(EntityWrapper<>);
      Type[] typeArgs = { Type.GetType(type.ToString()) };
      Type wrapperType = genericType.MakeGenericType(typeArgs);
      object processor = Activator.CreateInstance(wrapperType);
      var getEntity = wrapperType.GetMethod("GetEntity");

      var emptyEntity = (Entity)getEntity.Invoke(processor, new object[] { new string[0], new Dictionary<string, string>() });
      var paramCount = emptyEntity.GetPropertiesCount();

      var readerWriter = GetReaderWriter(logger, filePath, sheetName, extraParameters);

      // Пропускаем 1 строку с заголовками.
      var listImportItems = readerWriter.Read().Skip(1).ToList();
			var listResult = ImportEntities(logger, processor, getEntity, listImportItems, extraParameters);
			readerWriter.Write(listResult, paramCount);
    }

    public static List<List<Structures.ExceptionsStruct>> ImportEntities(Logger logger,
      object processor,
      MethodInfo getEntity,
      List<string[]> listImportItems,
      Dictionary<string, string> extraParameters)
    {
      logger.Info("======================Импорт сущностей=====================");
      var listResult = new List<List<Structures.ExceptionsStruct>>();
      var searchDoubles = extraParameters.ContainsKey(Constants.ExtraParameters.IgnoreDuplicates) ? extraParameters[Constants.ExtraParameters.IgnoreDuplicates] : string.Empty;
      var parametersListCount = listImportItems.Count;
      var watch = Stopwatch.StartNew();
      uint row = 2;
      uint rowImported = 1;
      foreach (var importItem in listImportItems)
      {
        var entity = (Entity)getEntity.Invoke(processor, new object[] { importItem.ToArray(), extraParameters });
        var exceptionList = new List<Structures.ExceptionsStruct>();
        var importItemCount = importItem.Length;
        if (entity != null)
        {
          if (importItemCount >= entity.GetPropertiesCount())
          {
            logger.Info($"Обработка сущности {row - 1}");
            watch.Restart();
            exceptionList = entity.SaveToRX(logger, searchDoubles).ToList();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            if (exceptionList.Any(x => x.ErrorType == Constants.ErrorTypes.Error))
            {
              logger.Info($"Сущность {row - 1} не импортирована");
            }
            else
            {
              logger.Info($"Сущность {row - 1} импортирована");
              logger.Info($"Времени затрачено на импорт сущности: {elapsedMs} мс");
              rowImported++;
            }
            row++;
          }
          else
          {
            var message = string.Format("Количества входных параметров недостаточно. " +
              "Количество ожидаемых параметров {0}. Количество переданных параметров {1}.", entity.GetPropertiesCount(), importItemCount);
            exceptionList.Add(new Structures.ExceptionsStruct { ErrorType = Constants.ErrorTypes.Error, Message = message });
            logger.Error(message);
          }
          listResult.Add(exceptionList);
        }
      }
      var percent1 = (double)(rowImported - 1) / (double)parametersListCount * 100.00;
      logger.Info($"\rИмпортировано {rowImported - 1} сущностей из {parametersListCount} ({percent1:F2}%)");

      return listResult;
    }

    private static IFileReaderWriter GetReaderWriter(Logger logger, string filePath, string sheetName, Dictionary<string, string> extraParameters)
    {
      var availableFormats = new List<string>() { "xlsx", "csv" };
			if (extraParameters.TryGetValue(Constants.ExtraParameters.InputFormat, out var inputFormat))
			{
				if (!availableFormats.Contains(inputFormat.ToLower()))
					throw new ArgumentException($"Нет обработчика для формата {inputFormat}");
			}

      if (!extraParameters.TryGetValue(Constants.ExtraParameters.CsvDelimiter, out var delimiter))
        delimiter = ";";

			return inputFormat?.ToLower() switch
			{
				"csv" => new CsvReaderWriter(logger, filePath, delimiter),
				_ => new ExcelReaderWriter(logger, filePath, sheetName),
			};
		}
  }
}
