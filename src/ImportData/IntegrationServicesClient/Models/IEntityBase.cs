using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.IntegrationServicesClient.Models
{
  public class IEntityBase
  {
    public long Id { get; set; }

    /// <summary>
    /// Поиск сущности для обновления или установки значения свойства в связанную сущность.
    /// </summary>
    /// <param name="propertiesForSearch">Поля со значениями для поиска сущности.</param>
    /// <param name="entity">Сущность со всеми параметрами загрузки. (Использовать, если необходимо написать сложную логику поиска. Могут быть заполнены не все поля.)</param>
    /// <param name="isEntityForUpdate">Вызов из обновляемой сущности (могут использоваться разные наборы полей для поиска).</param>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Найденная сущность.</returns>
    public static IEntityBase FindEntity(Dictionary<string, string> propertiesForSearch, Entity entity, bool isEntityForUpdate, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      return null;
    }

    /// <summary>
    /// Создание сущности с заполнением полей для установки значения свойства в связанную сущность.
    /// </summary>
    /// <param name="propertiesForSearch">Поля со значениями для создания сущности.</param>
    /// <param name="entity">Сущность со всеми параметрами загрузки. (Использовать, если необходимо написать сложную логику создания. Могут быть заполнены не все поля.)</param>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Созданная сущность.</returns>
    public static IEntityBase CreateEntity(Dictionary<string, string> propertiesForSearch, Entity entity, List<Structures.ExceptionsStruct> exceptionList, bool isBatch, NLog.Logger logger)
    {
      return null;
    }

    /// <summary>
    /// Создание / обновление сущности через OData.
    /// </summary>
    /// <param name="entity">Сущность.</param>
    /// <param name="isNewEntity">True, если сущность создается, false, если обновляется.</param>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="logger">Логировщик.</param>
    public static IEntityBase CreateOrUpdate(IEntityBase entity, bool isNewEntity, bool isBatch, List<Structures.ExceptionsStruct> exceptionList, NLog.Logger logger)
    {
      return null;
    }

    /// <summary>
    /// Преобразовать в DateTimeOffset строку.
    /// </summary>
    /// <param name="regDate">Строка для преобразования.</param>
    /// <returns>Результат преобразования.</returns>
    public static bool GetDate(string regDate, out DateTimeOffset registrationDate)
    {
      if (double.TryParse(regDate, out var date))
      {
        registrationDate = new DateTimeOffset(DateTime.FromOADate(date), TimeSpan.Zero);
        // HACK: СИ работает с учетом часового пояса, и документы хранит с учетом поправки на часовой пояс, приведем дату к часовому поясу рабочего места.
        registrationDate = DateTimeOffset.Parse(registrationDate.ToString("yyyy-MM-dd"));
        return true;
      }
      registrationDate = DateTimeOffset.MinValue;
      return false;
    }

    /// <summary>
    /// Преобразовать в DateTimeOffset строку.
    /// </summary>
    /// <param name="regDate">Строка для преобразования.</param>
    /// <returns>Результат преобразования.</returns>
    public static DateTimeOffset GetDate(string regDate)
    {
      DateTimeOffset registrationDate = DateTimeOffset.MinValue;
      if (double.TryParse(regDate, out var date))
      {
        registrationDate = new DateTimeOffset(DateTime.FromOADate(date), TimeSpan.Zero);
        registrationDate = DateTimeOffset.Parse(registrationDate.ToString("yyyy-MM-dd"));
        return registrationDate;
      }
      return registrationDate;
    }
  }
}
