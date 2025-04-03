using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportData
{
  public class EntityWrapper<T> where T : Entity, new()
  {
    /// <summary>
    /// Получить generic сущность.
    /// </summary>
    /// <param name="parameters">Список параметров для сущности.</param>
    /// <returns>Generic сущность.</returns>
    public T GetEntity(string[] parameters, Dictionary<string, string> extraParameters)
    {
      var entity = (T)Activator.CreateInstance(typeof(T));
      entity.ExtraParameters = extraParameters;
      return entity;
    }
  }
}
