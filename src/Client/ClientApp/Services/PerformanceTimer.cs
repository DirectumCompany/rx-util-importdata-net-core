using System;
using System.Diagnostics;

namespace ClientApp.Services;

/// <summary>
/// Логгирование времени выполнения методов.
/// </summary>
public class PerformanceTimer : IDisposable
{
  private readonly Stopwatch Timer = new Stopwatch();

  private readonly string methodName;

  /// <summary>
  /// Конструктор.
  /// </summary>
  public PerformanceTimer(string methodName)
  {
    this.methodName = methodName;
    this.Timer.Start();
  }

  /// <summary>
  /// Логирование в консоль.
  /// </summary>
  public static void Log(string methodName, Stopwatch timer)
  {
    if (timer == null)
      return;
    Console.WriteLine($"Operation: {methodName}. {timer.ElapsedMilliseconds} ms.");
  }

  /// <inheritdoc />
  public void Dispose()
  {
    this.Timer.Stop();
    Log(methodName, Timer);
  }
}
