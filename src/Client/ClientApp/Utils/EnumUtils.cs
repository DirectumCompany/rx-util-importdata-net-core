#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Linq;

namespace ClientApp.Utils;

/// <summary>
/// Вспомогательный класс для работы со значениями перечислений.
/// </summary>
public static class EnumUtils
{
  public static string CoerceValue(IDictionary<string, string> enumValues, string value)
  {
    enumValues ??= new Dictionary<string, string>();
    if (string.IsNullOrEmpty(value))
    {
      return enumValues.FirstOrDefault(x => string.IsNullOrEmpty(x.Value)).Key != null ? value : enumValues.FirstOrDefault().Value;
    }

    var enumValue = enumValues.FirstOrDefault(x => string.Equals(x.Value, value, StringComparison.OrdinalIgnoreCase));
    return enumValue.Value ?? value;
  }
}
