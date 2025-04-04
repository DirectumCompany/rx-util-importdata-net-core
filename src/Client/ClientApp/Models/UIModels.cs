#pragma warning disable CS1591

using System.Collections.Generic;
using System.Linq;

namespace ClientApp.Models
{
  /// <summary>
  /// Переменная, с которой можно работать в UI.
  /// </summary>
  public class UIVariable
  {
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Control { get; set; }
    public string Description { get; set; }
    public object Value { get; set; }
    public Dictionary<string, string> EnumValues { get; set; } = new();
    public ControlState State { get; set; } = new();
  }

  public static class UIControls
  {
    public static string BooleanControl { get; } = "boolean_string_control";
    public static string PasswordControl { get; } = "password_control";
    public static string EnumControl { get; } = "enum_control";
    public static string DirectoryControl { get; } = "directory_control";
    public static string FileControl { get; } = "file_control";
    public static string MultipleFileControl { get; } = "multiple_file_control";
    public static string GroupHeaderControl { get; } = "group_header_control";
    public static string StringControl { get; } = "string_control";
    public static string EmptyControl { get; } = "empty_control";
  }

  public static class UIControlsName
	{
      public static string ImportUtil { get; } = "importUtil";
      public static string Action { get; } = "action";
      public static string ImportPackagePath { get; } = "import_package_path";
      public static string Login { get; } = "login";
      public static string Password { get; } = "password";
      public static string SearchDoubles { get; } = "search_doubles";
      public static string UpdateBody { get; } = "update_body";
      public static string DocRegisterId { get; } = "doc_register_id";
      
      public static string BatchRequest { get; } = "batch_request";
  }
}
