#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientApp.Utils
{
  /// <summary>
  /// Хэлпер для обработки строк.
  /// </summary>
  public static class StringUtils
  {
    /// <summary>
    /// Заменить символы переноса строк.
    /// </summary>
    /// <param name="value">Строка.</param>
    /// <param name="replaceValue">На что заменить.</param>
    /// <returns>Строка без символов перевода строк.</returns>
    public static string ReplaceNewLines(this string value, string replaceValue)
    {
      if (string.IsNullOrEmpty(value))
        return value;

      return System.Text.RegularExpressions.Regex.Replace(value, "\r?\n|\r|\v", replaceValue ?? string.Empty);
    }

    public static string ReplaceFirstOccurrence(this string source, string find, string replace)
    {
      if (string.IsNullOrWhiteSpace(source) || find == null || replace == null)
        return source;

      if (!source.StartsWith(find, StringComparison.Ordinal))
        return source;

      return source.Remove(0, find.Length).Insert(0, replace);
    }

    public static string ToBase64(string rawData)
    {
      if (string.IsNullOrEmpty(rawData))
        return rawData;

      return System.Convert.ToBase64String(Encoding.UTF8.GetBytes(rawData));
    }

    public static string DirListToString(List<string> list, string separator)
    {
      var trimmedNonEmptyList = list.Where(i => !string.IsNullOrWhiteSpace(i)).Select(i => i.TrimEnd('/', '\\'));
      return string.Join(separator, trimmedNonEmptyList);
    }
  }
}
