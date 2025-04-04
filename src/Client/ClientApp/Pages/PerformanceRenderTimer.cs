using System;
using System.Diagnostics;
using ClientApp.Services;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Pages;

/// <summary>
/// Логгирование времени рендера компонент.
/// </summary>
public class PerformanceRenderTimer : ComponentBase
{
  private Stopwatch RenderTimer;

  /// <inheritdoc />
  protected override void OnInitialized()
  {
    base.OnInitialized();
    RenderTimer = Stopwatch.StartNew();
  }

  /// <inheritdoc />
  protected override bool ShouldRender()
  {
    RenderTimer = Stopwatch.StartNew(); // all re-renders (not the first render)
    return base.ShouldRender();
  }

  /// <inheritdoc />
  protected override void OnAfterRender(bool firstRender)
  {
    base.OnAfterRender(firstRender);
    RenderTimer?.Stop();
    PerformanceTimer.Log($"render {GetType().Name}", RenderTimer);
  }
}
