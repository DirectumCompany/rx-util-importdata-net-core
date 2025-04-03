using System.Globalization;
using System.Security.Cryptography;
using DocumentFormat.OpenXml.VariantTypes;
using ImportData;
using ImportData.Entities.Databooks;
using ImportData.IntegrationServicesClient.Models;
using System.Linq;
using Xunit.Extensions.Ordering;
using Microsoft.VisualBasic;

namespace Tests.EDocs
{

  public partial class Tests
  {
    [Fact, Order(5)]
    public void T0_CaseFileImport()
    {
      var xlsxPath = TestSettings.CaseFilePathXlsx;
      var action = ImportData.Constants.Actions.ImportCaseFiles;
      var sheetName = ImportData.Constants.SheetNames.CaseFiles;

      var items = Common.XlsxParse(xlsxPath, sheetName);

      Program.Main(Common.GetArgs(action, xlsxPath));

      var errorList = new List<string>();
      foreach (var caseFile in items)
      {
        var error = EqualsCaseFile(caseFile);

        if (string.IsNullOrEmpty(error))
          continue;

        errorList.Add(error);
      }

      if (errorList.Any())
        Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, errorList));
    }

    public static string EqualsCaseFile(List<string> parameters, int shift = 0)
    {
      var exceptionList = new List<Structures.ExceptionsStruct>();
      var index = parameters[shift].Trim();
      var title = parameters[shift+1].Trim();

      var caseFile = BusinessLogic.GetEntityWithFilter<ICaseFiles>(x => x.Index == index && x.Title == title, exceptionList, TestSettings.Logger, true);

      if (caseFile == null)
        return $"Не найдена номенклатура дел: \"{index}. {title}\"";

      var errorList = new List<string>
            {
                Common.CheckParam(caseFile.Index, parameters[shift + 0], "Index"),
                Common.CheckParam(caseFile.Title, parameters[shift + 1], "Title"),
                Common.CheckParam(caseFile.RetentionPeriod?.Name, parameters[shift + 2], "RetentionPeriodName"),
                Common.CheckParam(caseFile.RetentionPeriod?.RetentionPeriod, int.TryParse(parameters[shift + 3], out var period) ? period.ToString() : string.Empty, "RetentionPeriod"),
                Common.CheckParam(caseFile.StartDate, parameters[shift + 4], "StartDate"),
                Common.CheckParam(caseFile.EndDate, parameters[shift + 5], "EndDate"),
                Common.CheckParam(caseFile.BusinessUnit?.Name, parameters[shift + 6], "BusinessUnit"),
                Common.CheckParam(caseFile.Department?.Name, parameters[shift + 7], "Department"),
                Common.CheckParam(caseFile.RegistrationGroup?.Name, parameters[shift + 8], "RegistrationGroup"),
                Common.CheckParam(caseFile.Note, parameters[shift + 9], "Note"),
            };

      errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
      if (errorList.Any())
        errorList.Insert(0, $"Ошибка в сущности: \"{index}. {title}\"");

      return string.Join(Environment.NewLine, errorList);
    }
  }
}
