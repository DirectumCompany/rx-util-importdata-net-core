using ImportData;
using Xunit.Extensions.Ordering;

namespace Tests.Databooks
{
  [Order(10)]
  public partial class Tests
  {
    [Fact, Order(10)]
    public void T1_CompanyImport()
    {
      var xlsxPath = TestSettings.CompanyPathXlsx;
      var action = ImportData.Constants.Actions.ImportCompany;
      var sheetNameEmployees = ImportData.Constants.SheetNames.Employees;
      var sheetNameBusinessUnits = ImportData.Constants.SheetNames.BusinessUnits;
      var sheetNameDepartments = ImportData.Constants.SheetNames.Departments;

      Program.Main(Common.GetArgs(action, xlsxPath));

      var errorList = new List<string>();
      //Проверка работников.
      foreach (var expectedEmployee in Common.XlsxParse(xlsxPath, sheetNameEmployees))
      {
        var error = EqualsEmployee(expectedEmployee);

        if (string.IsNullOrEmpty(error))
          continue;

        errorList.Add(error);
      }

      //Проверка наших организаций.
      foreach (var expectedBusinessUnit in Common.XlsxParse(xlsxPath, sheetNameBusinessUnits))
      {
        var error = EqualsBusinessUnit(expectedBusinessUnit);

        if (string.IsNullOrEmpty(error))
          continue;

        errorList.Add(error);
      }

      //Проверка подразделений.
      foreach (var expectedDepartment in Common.XlsxParse(xlsxPath, sheetNameDepartments))
      {
        var error = EqualsDepartaments(expectedDepartment);

        if (string.IsNullOrEmpty(error))
          continue;

        errorList.Add(error);
      }

      if (errorList.Any())
        Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, errorList));
    }

    public static string EqualsEmployee(List<string> parameters, int shift = 0)
    {
      var exceptionList = new List<Structures.ExceptionsStruct>();
      var name = string.Join(' ', parameters[shift + 3].Trim(), parameters[shift + 4].Trim(), parameters[shift + 5].Trim()).Trim();
      var actualEmployee = BusinessLogic.GetEntityWithFilter<IEmployees>(x => x.Name == name, exceptionList, TestSettings.Logger, true);
      var actualPerson = BusinessLogic.GetEntityWithFilter<IPersons>(x => x.Name == name, exceptionList, TestSettings.Logger, true);
      if (actualEmployee == null)
        return $"Не найден сотрудник {name}";
      if (actualPerson == null)
        return $"Не найдена персона {name}";

      var errorList = new List<string>
      {
        Common.CheckParam(actualEmployee.Department, parameters[shift + 1], "Department"),
        Common.CheckParam(actualEmployee.JobTitle?.Name, parameters[shift + 2], "JobTitle"),
        Common.CheckParam(actualPerson.LastName, parameters[shift + 3], "LastName"),
        Common.CheckParam(actualPerson.FirstName, parameters[shift + 4], "FirstName"),
        Common.CheckParam(actualPerson.MiddleName, parameters[shift + 5], "MiddleName"),
        Common.CheckParam(actualPerson.Sex, BusinessLogic.GetPropertySex(parameters[shift + 6]), "Sex"),
        Common.CheckParam(actualPerson.DateOfBirth, parameters[shift + 7], "DateOfBirth"),
        Common.CheckParam(actualPerson.TIN, parameters[shift + 8], "TIN"),
        Common.CheckParam(actualPerson.INILA, parameters[shift + 9], "INILA"),
        Common.CheckParam(actualPerson.City?.Name, parameters[shift + 10], "City"),
        Common.CheckParam(actualPerson.Region?.Name, parameters[shift + 11], "Region"),
        Common.CheckParam(actualPerson.LegalAddress, parameters[shift + 12], "LegalAddress"),
        Common.CheckParam(actualPerson.PostalAddress, parameters[shift + 13], "PostalAddress"),
        Common.CheckParam(actualEmployee.Phone, parameters[shift + 14], "Phone"),
        Common.CheckParam(actualEmployee.Email, parameters[shift + 15], "Email"),
        Common.CheckParam(actualPerson.Homepage, parameters[shift + 16], "Homepage"),
        Common.CheckParam(actualPerson.Bank?.Name.ToLower(), parameters[shift + 17].ToLower(), "Bank"),
        Common.CheckParam(actualPerson.Account, parameters[shift + 18], "Account"),
        Common.CheckParam(actualEmployee.Note, parameters[shift + 19], "Note"),

        Common.CheckParam(actualEmployee.NeedNotifyExpiredAssignments != null ? actualEmployee.NeedNotifyExpiredAssignments.ToString() : (false).ToString(),
          string.IsNullOrEmpty(parameters[shift + 15]) ? (false).ToString() : (true).ToString(), "NeedNotifyExpiredAssignments"),
        Common.CheckParam(actualEmployee.NeedNotifyNewAssignments != null ? actualEmployee.NeedNotifyExpiredAssignments.ToString() : (false).ToString(),
          string.IsNullOrEmpty(parameters[shift + 15]) ? (false).ToString() : (true).ToString(), "NeedNotifyNewAssignments"),
        Common.CheckParam(actualEmployee.NeedNotifyAssignmentsSummary != null ? actualEmployee.NeedNotifyExpiredAssignments.ToString() : (false).ToString(),
          string.IsNullOrEmpty(parameters[shift + 15]) ? (false).ToString() : (true).ToString(), "NeedNotifyAssignmentsSummary"),
      };

      errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
      if (errorList.Any())
        errorList.Insert(0, $"Ошибка в сущности: {name}");

      return string.Join(Environment.NewLine, errorList);
    }

    public static string EqualsBusinessUnit(List<string> parameters, int shift = 0)
    {
      var exceptionList = new List<Structures.ExceptionsStruct>();
      var name = parameters[shift + 0].Trim();
      var actualBusinessUnit = BusinessLogic.GetEntityWithFilter<IBusinessUnits>(x => x.Name == name, exceptionList, TestSettings.Logger, true);

      if (actualBusinessUnit == null)
        return $"Не найдена наша организация {name}";

      var errorList = new List<string>
            {
                Common.CheckParam(actualBusinessUnit.Name, parameters[shift + 0], "Name"),
                Common.CheckParam(actualBusinessUnit.LegalName, parameters[shift + 1], "LegalName"),
                Common.CheckParam(actualBusinessUnit.HeadCompany, parameters[shift + 2], "HeadCompany"),
                Common.CheckParam(actualBusinessUnit.CEO, parameters[shift + 3], "CEO"),
                Common.CheckParam(actualBusinessUnit.Nonresident.ToString(), (parameters[shift + 4].Trim().ToLower() == "да").ToString(), "Nonresident"),
                Common.CheckParam(actualBusinessUnit.TIN, parameters[shift + 5], "TIN"),
                Common.CheckParam(actualBusinessUnit.TRRC, parameters[shift + 6], "TRRC"),
                Common.CheckParam(actualBusinessUnit.PSRN, parameters[shift + 7], "PSRN"),
                Common.CheckParam(actualBusinessUnit.NCEO, parameters[shift + 8], "NCEO"),
                Common.CheckParam(actualBusinessUnit.NCEA, parameters[shift + 9], "NCEA"),
                Common.CheckParam(actualBusinessUnit.City?.Name, parameters[shift + 10], "City"),
                Common.CheckParam(actualBusinessUnit.Region?.Name, parameters[shift + 11], "Region"),
                Common.CheckParam(actualBusinessUnit.LegalAddress, parameters[shift + 12], "LegalAddress"),
                Common.CheckParam(actualBusinessUnit.PostalAddress, parameters[shift + 13], "PostalAddress"),
                Common.CheckParam(actualBusinessUnit.Phones, parameters[shift + 14], "Phones"),
                Common.CheckParam(actualBusinessUnit.Email, parameters[shift + 15], "Email"),
                Common.CheckParam(actualBusinessUnit.Homepage, parameters[shift + 16], "Homepage"),
                Common.CheckParam(actualBusinessUnit.Note, parameters[shift + 17], "Note"),
                Common.CheckParam(actualBusinessUnit.Account, parameters[shift + 18], "Account"),
                Common.CheckParam(actualBusinessUnit.Bank, parameters[shift + 19], "Bank")
            };

      errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
      if (errorList.Any())
        errorList.Insert(0, $"Ошибка в сущности: {name}");

      return string.Join(Environment.NewLine, errorList);
    }

    public static string EqualsDepartaments(List<string> parameters, int shift = 0)
    {
      var exceptionList = new List<Structures.ExceptionsStruct>();
      var name = parameters[shift + 0].Trim();
      var actualDepartment = BusinessLogic.GetEntityWithFilter<IDepartments>(x => x.Name == name, exceptionList, TestSettings.Logger, true);

      if (actualDepartment == null)
        return $"Не найдено подразделение {name}";

      var errorList = new List<string>
            {
                Common.CheckParam(actualDepartment.Name, parameters[shift + 0], "Name"),
                Common.CheckParam(actualDepartment.ShortName, parameters[shift + 1], "ShortName"),
                Common.CheckParam(actualDepartment.Code, parameters[shift + 2], "Code"),
                Common.CheckParam(actualDepartment.BusinessUnit, parameters[shift + 3], "BusinessUnit"),
                Common.CheckParam(actualDepartment.HeadOffice, parameters[shift + 4], "HeadOffice"),
                Common.CheckParam(actualDepartment.Manager , parameters[shift + 5], "Manager"),
                Common.CheckParam(actualDepartment.Phone, parameters[shift + 6], "Phone"),
                Common.CheckParam(actualDepartment.Note, parameters[shift + 7], "Note")
            };

      errorList = errorList.Where(x => !string.IsNullOrEmpty(x)).ToList();
      if (errorList.Any())
        errorList.Insert(0, $"Ошибка в сущности: {name}");

      return string.Join(Environment.NewLine, errorList);
    }
  }
}