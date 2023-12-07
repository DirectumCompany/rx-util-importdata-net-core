using System;
using System.Collections.Generic;
using System.Text;

namespace ImportData.Logic
{
  internal interface IFileReaderWriter
  {
    public List<string[]> Read();
    public void Write(List<List<Structures.ExceptionsStruct>> listResult, int paramCount);
  }
}
