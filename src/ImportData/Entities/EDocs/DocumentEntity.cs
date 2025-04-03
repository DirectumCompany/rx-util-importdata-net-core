using DocumentFormat.OpenXml.Wordprocessing;
using ImportData.IntegrationServicesClient.Models;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImportData.Entities.EDocs
{
  public class DocumentEntity : Entity
  {
    /// <summary>
    /// Признак сущностей, для которых требуется тело документа.
    /// </summary>
    protected virtual bool RequiredDocumentBody { get { return false; } }

    public override IEnumerable<Structures.ExceptionsStruct> SaveToRX(NLog.Logger logger, string ignoreDuplicates, bool isBatch = false)
    {
      var exceptionList = new List<Structures.ExceptionsStruct>();

      // Перед обработкой сущности проверим, что в шаблоне есть обязательное поле "файл", и указанный по пути файл существует.
      if (CheckNeedRequiredDocumentBody(EntityType, out var exceptions))
      {
        if (exceptions.Count > 0)
        {
          exceptionList.AddRange(exceptions);
          return exceptionList;
        }
      }

      exceptionList.AddRange(base.SaveToRX(logger, ignoreDuplicates));

      // Импорт тела документа в систему.
      if (NamingParameters.ContainsKey(Constants.CellNameFile) && isNewEntity)
      {
        var filePath = NamingParameters[Constants.CellNameFile];
        var update_body = ExtraParameters.ContainsKey("update_body") && ExtraParameters["update_body"] == "true";

        if (!string.IsNullOrWhiteSpace(filePath) && entity != null)
          exceptionList.AddRange(BusinessLogic.ImportBody((IElectronicDocuments)entity, filePath, logger, update_body, isBatch));
      }

      return exceptionList;
    }

    protected override string GetName()
    {
      var documentKind = ResultValues[Constants.KeyAttributes.DocumentKind];
      var subject = ResultValues[Constants.KeyAttributes.Subject];

      return string.Format("{0} \"{1}\"", documentKind, subject);
    }

    protected override bool FillProperies(List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      ResultValues[Constants.KeyAttributes.Name] = GetName();
      ResultValues[Constants.KeyAttributes.Created] = (DateTimeOffset?)ResultValues[Constants.KeyAttributes.RegistrationDate];
      ResultValues[Constants.KeyAttributes.RegistrationState] = BusinessLogic.GetRegistrationsState((string)ResultValues[Constants.KeyAttributes.RegistrationState]);

      if (ResultValues.ContainsKey(Constants.KeyAttributes.LifeCycleState) && ResultValues[Constants.KeyAttributes.LifeCycleState] != null)
        ResultValues[Constants.KeyAttributes.LifeCycleState] = BusinessLogic.GetPropertyLifeCycleState((string)ResultValues[Constants.KeyAttributes.LifeCycleState]);
      else
        ResultValues[Constants.KeyAttributes.LifeCycleState] = BusinessLogic.GetPropertyLifeCycleState(Constants.AttributeValue[Constants.KeyAttributes.Status]);

      return false;
    }

    /// <summary>
    /// Проверка требования наличия пути к телу документа и самого документа по пути.
    /// </summary>
    /// <param name="entityType">Сущность RX для заполнения.</param>
    /// <param name="exceptionList">Список исключений.</param>
    /// <returns>Результат проверки.</returns>
    private bool CheckNeedRequiredDocumentBody(Type entityType, out List<Structures.ExceptionsStruct> exceptionList)
    {
      exceptionList = new List<Structures.ExceptionsStruct>();
      if (RequiredDocumentBody)
      {
        if (NamingParameters.ContainsKey(Constants.CellNameFile))
        {
          var pathToBody = NamingParameters[Constants.CellNameFile];
          if (string.IsNullOrWhiteSpace(pathToBody))
          {
            exceptionList.Add(new Structures.ExceptionsStruct
            {
              ErrorType = Constants.ErrorTypes.Error,
              Message = string.Format(Constants.Resources.EmptyColumn, Constants.CellNameFile)
            });
          }
          if (!System.IO.File.Exists(pathToBody))
            exceptionList.Add(new Structures.ExceptionsStruct
            {
              ErrorType = Constants.ErrorTypes.Error,
              Message = string.Format(Constants.Resources.FileNotExist, pathToBody)
            });
        }
        else
          exceptionList.Add(new Structures.ExceptionsStruct
          {
            ErrorType = Constants.ErrorTypes.Error,
            Message = string.Format(Constants.Resources.NeedRequiredDocumentBody, Constants.CellNameFile)
          });
        return true;
      }
      else
        return false;
    }
  }
}