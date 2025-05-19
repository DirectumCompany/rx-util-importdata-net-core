#pragma warning disable CS1591

namespace ClientApp.Models;

/// <summary>
/// Локализованные сообщения.
/// </summary>
public class ClientAppMessages
{
  // setter - обязателен. иначе будет пустая строка при десериализации.
  public string Title { get; set; }
  public string ImportButton { get; set; }
  public string ImportModeButton { get; set; }
  public string ImportInfo { get; set; }
  public string RequiredError { get; set; }
  public string ChangeBlocked { get; set; }
}
