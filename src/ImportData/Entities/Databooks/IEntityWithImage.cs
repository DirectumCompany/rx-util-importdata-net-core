using ImportData.IntegrationServicesClient.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.Entities.Databooks
{
  /// <summary>
  /// Интерфейс для добавления картинок для сущностей.
  /// </summary>
  public interface IEntityWithImage
  {
    /// <summary>
    /// Добавить картинку для сущности.
    /// </summary>
    /// <param name="image">Картинка.</param>
    /// <returns>Результат добавления картинки.</returns>
    bool AddImage(IBinaryData image);
  }
}