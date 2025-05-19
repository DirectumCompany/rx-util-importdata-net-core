#pragma warning disable CS1591

using System;
using System.Threading.Tasks;

namespace ClientApp.Services
{
  public interface IRefreshService
  {
    event Action RefreshRequested;
    event Action UIConfigureRefreshRequested;

    void CallRequestRefresh();
    void CallUIConfigureRequestRefresh();
  }

  /// <summary>
  /// Сервис для добавления и вызова action'ов.
  /// </summary>
  public class RefreshService: IRefreshService
  {
    public event Action RefreshRequested;
    public event Action UIConfigureRefreshRequested;

    public void CallRequestRefresh()
    {
      this.RefreshRequested?.Invoke();
    }

    public void CallUIConfigureRequestRefresh()
    {
      this.UIConfigureRefreshRequested?.Invoke();
    }
  }
}
