using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Word;
using ImportData;
using ImportData.IntegrationServicesClient.Models;
using Xunit.Extensions.Ordering;

namespace Tests.EDocs
{
  public partial class Tests
  {
    [Fact, Order(20)]
    public void T2_AddendumsImport()
    {
      var xlsxPath = TestSettings.AddendumsPathXlsx;
      var action = ImportData.Constants.Actions.ImportAddendums;
      var sheetName = ImportData.Constants.SheetNames.Addendums;

      var items = Common.XlsxParse(xlsxPath, sheetName);

      Program.Main(Common.GetArgs(action, xlsxPath));

      var errorList = new List<string>();
      foreach (var expectedAddendum in items)
      {
        var error = EqualsAddendums(expectedAddendum);

        if (string.IsNullOrEmpty(error))
          continue;

        errorList.Add(error);
      }
      if (errorList.Any())
        Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, errorList));
    }

    public static string EqualsAddendums(List<string> parameters, int shift = 0)
    {
      var exceptionList = new List<Structures.ExceptionsStruct>();
      var docDate = IEntityBase.GetDate(parameters[shift + 3].Trim());
      var docKind = parameters[shift + 5].Trim();
      var subject = parameters[shift + 6].Trim();
      var leadingDocumentNumber = parameters[shift + 2].Trim();
      var name = $"{docKind} \"{subject}\"";
      
      var leadingDocument = BusinessLogic.GetEntityWithFilter<IContracts>(x => x.RegistrationNumber != null &&
          x.RegistrationNumber == leadingDocumentNumber &&
          x.DocumentDate == docDate, exceptionList, TestSettings.Logger, true);

      var actualAddendum = BusinessLogic.GetEntityWithFilter<IAddendums>(c => c.LeadingDocument.Id == leadingDocument.Id &&
                                                                                    c.DocumentKind.Name == docKind &&
                                                                                    c.Subject == subject,
                                                                                    exceptionList, TestSettings.Logger, true);
      if (actualAddendum == null)
        return $"Не найдено приложение: {name}";

      var errorList = new List<string>
            {
                //Common.CheckParam(actualAddendum.RegistrationNumber, parameters[shift + 0], "RegistrationNumber"),
                //Common.CheckParam(actualAddendum.RegistrationDate, parameters[shift + 1], "RegistrationDate"),
                //Common.CheckParam(actualAddendum.Counterparty, parameters[shift + 4], "Counterparty"),
                Common.CheckParam(leadingDocument.RegistrationNumber, parameters[shift + 2], "LeadingDocumentRegNumber"),
                Common.CheckParam(leadingDocument.RegistrationDate, parameters[shift + 3], "LeadingDocumentRegNumberRegDate"),
                Common.CheckParam(actualAddendum.DocumentKind, parameters[shift + 5], "DocumentKind"),
                Common.CheckParam(actualAddendum.Subject, parameters[shift + 6], "Subject"),
                Common.CheckParam(leadingDocument.BusinessUnit?.Name, parameters[shift + 7], "BusinessUnit"),
                Common.CheckParam(actualAddendum.Department, parameters[shift + 8], "Department"),
                Common.CheckParam(actualAddendum.LastVersion(), parameters[shift + 9], "LastVersion"),
                Common.CheckParam(actualAddendum.LifeCycleState, BusinessLogic.GetPropertyLifeCycleState(parameters[shift + 10]), "LifeCycleState"),
                //Common.CheckParam(actualAddendum.ResponsibleEmployee, parameters[shift + 11], "ResponsibleEmployee"),
                Common.CheckParam(actualAddendum.OurSignatory, parameters[shift + 12], "OurSignatory"),
                Common.CheckParam(actualAddendum.Note, parameters[shift + 13], "Note"),
                //Common.CheckParam(actualAddendum.DocumentRegister?.Id, parameters[shift + 14], "DocumentRegister"),
                //Common.CheckParam(actualAddendum.RegistrationState, BusinessLogic.GetRegistrationsState(parameters[shift + 15]), "RegistrationState")
            };

      errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
      if (errorList.Any())
        errorList.Insert(0, $"Ошибка в сущности: {name}");

      return string.Join(Environment.NewLine, errorList);
    }
  }
}
