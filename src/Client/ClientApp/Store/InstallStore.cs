#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ClientApp.Api;
using ClientApp.Models;

namespace ClientApp.Store
{
  /// <summary>
  /// Глобальный store для хранения состояния установки и обновления.
  /// </summary>
  public class InstallStore
  {
    private readonly CommonApi apiClient;

    public IList<UIVariable> UIVariables { get; private set; } = new List<UIVariable>();


    public bool ValidationInProgress { get; set; }

    public string Title { get; private set; } = "";

    public static int SnackbarVisibleStateDuration => 60000;

    public static int SnackbarDisappearedTime => 3000;

    public List<string> ImportOutputdata { get; private set; } = new List<string>();

    public void GetUIVariables()
    {
      var uiVariables = new List<UIVariable>
      {
        new UIVariable() { Name = UIControlsName.ImportUtil, Control = "group_header_control", DisplayName = "Утилита импорта", Description = "{\"name\": \"Утилита импорта\"}" },
        new UIVariable() { Name = UIControlsName.Action, Control = "enum_control", DisplayName = "Действие", EnumValues = new () {
            { "ImportCompany", "ImportCompany" },
            { "ImportCompanies", "ImportCompanies" },
            { "ImportPersons", "ImportPersons" },
            { "ImportContracts", "ImportContracts" },
            { "ImportSupAgreements", "ImportSupAgreements" },
            { "ImportIncomingLetters", "ImportIncomingLetters" },
            { "ImportOutgoingLetters", "ImportOutgoingLetters" },
            { "ImportOutgoingLettersAddressees", "ImportOutgoingLettersAddressees" },
            { "ImportOrders", "ImportOrders" },
            { "ImportAddendums", "ImportAddendums" },
            { "ImportWaybills", "ImportWaybills" },
            { "ImportUTD", "ImportUTD" },
            { "ImportContractStatement", "ImportContractStatement" },
            { "ImportDepartments", "ImportDepartments" },
            { "ImportEmployees", "ImportEmployees" },
            { "ImportContacts", "ImportContacts" },
            { "ImportLogins", "ImportLogins" },
            { "ImportSubstitutions", "ImportSubstitutions" },
            { "ImportCompanyDirectives", "ImportCompanyDirectives" },
            { "ImportCaseFiles", "ImportCaseFiles" }
            }
        },
        new UIVariable() { Name = UIControlsName.ImportPackagePath, Control = "file_control", DisplayName = "Путь до файла", Value = @"Template\Example\Договоры.xlsx", Description = "Укажите полный путь до файла импорта", NeedQuotes = true },
        new UIVariable() { Name = UIControlsName.Login, Control = "string_control", DisplayName = "Логин", Value = "Administrator", NeedQuotes = true },
        new UIVariable() { Name = UIControlsName.Password, Control = "password", DisplayName = "Пароль", Value = "11111" },
        new UIVariable() { Name = UIControlsName.SearchDoubles, Control = "boolean_string_control", DisplayName = "Признак поиска дубликатов", Value = "true", State = new ControlState { IsRequired = false } },
        new UIVariable() { Name = UIControlsName.UpdateBody, Control = "boolean_string_control", DisplayName = "Признак обновления последней версии документа", State = new ControlState { IsRequired = false } },
        new UIVariable() { Name = UIControlsName.BatchRequest, Control = UIControls.BooleanControl, DisplayName = "Пакетный запрос", State = new ControlState { IsRequired = false } },
        new UIVariable() { Name = UIControlsName.DocRegisterId, Control = "string_control", DisplayName = "Журнал регистрации", State = new ControlState { IsRequired = false } }
      };

      this.UIVariables = uiVariables;
    }

    public string Style()
    {
      return $"height: calc(100vh - 1px); overflow-y: hidden; background: no-repeat right top url('{Images.Images.Background}')";
    }

    public string ContentStyle()
    {
      return "height: calc(100% - 40px); overflow: auto;";
    }

    public ClientAppMessages Messages { get; private set; } = new();

    public void LoadLcMessages()
    {
      if (string.IsNullOrEmpty(this.Messages.ImportButton))
      {
        this.Messages.ImportButton = "Импорт";
        this.Messages.ImportModeButton = "Импорт";
        this.Messages.ImportInfo = "Информация импорта";
        this.Messages.Title = "Утилита импорта";
        this.Messages.RequiredError = "Заполните поле";
      }
    }

    public void SetTitle()
    {
      if (string.IsNullOrEmpty(this.Title))
      {
        this.Title = this.Messages.Title;
      }
    }

    public async Task Import(string arguments)
    {
      Console.WriteLine($"Operation: Import");
      await this.apiClient.GetAsync($"api/import?arguments={arguments}"); ;
    }

    public async Task GetOutputData()
    {
      Console.WriteLine($"Operation: GetOutputData");
      var outputData = await this.apiClient.GetFromJsonAsync<IList<string>>("api/getoutputdata");
      Console.WriteLine($"Operation: GetOutputData. line {string.Join(',', outputData)}");
      ImportOutputdata.AddRange(outputData);
    }

    public async Task<bool> IsImportActive()
    {
      Console.WriteLine($"Operation: IsImportActive");
      var result = await this.apiClient.GetFromJsonAsync<bool>("api/isactive");
      Console.WriteLine($"Operation: IsImportActive. result {result}");
      return result;
    }
    public string CreateArguments()
    {
      var result = new List<string>();

      Console.WriteLine($"Operation: CreateArguments.");
      AddArgument("-n", UIControlsName.Login, result);
      AddArgument("-p", UIControlsName.Password, result);
      AddArgument("-a", UIControlsName.Action, result);
      AddArgument("-f", UIControlsName.ImportPackagePath, result);
      AddArgument("-dr", UIControlsName.DocRegisterId, result);
      AddArgument("-d", UIControlsName.SearchDoubles, result);
      AddArgument("-ub", UIControlsName.UpdateBody, result);
      AddArgument("--batch", UIControlsName.BatchRequest, result);

      var resultStr = string.Join(' ', result);
      Console.WriteLine($"Operation: CreateArguments. result \"{resultStr}\"");
      return resultStr;
    }

    public void AddArgument(string actionKey, string controlName, IList<string> result)
    {
      Console.WriteLine($"Operation: AddArgument. actionKey {actionKey} controlName {controlName}");
      var controlValue = UIVariables.FirstOrDefault(v => v.Name == controlName)?.GetStringValue();
      if (!string.IsNullOrEmpty(controlValue))
      {
        result.Add(actionKey);
        result.Add(controlValue);
      }
    }

    public InstallStore(CommonApi client)
    {
      this.apiClient = client;
    }
  }
}
