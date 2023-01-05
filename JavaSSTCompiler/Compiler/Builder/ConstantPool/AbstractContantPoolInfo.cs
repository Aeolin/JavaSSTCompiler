using JavaSST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.ConstantPool
{
  public class AbstractConstantPoolInfo : AbstractSerializable
  {
    [Field(-1, typeof(byte))]
    public ConstantPoolTag Tag { get; init; }

    public ushort Index { get; init; }

    public AbstractConstantPoolInfo(ConstantPoolTag tag, ushort index)
    {
      Tag = tag;
      Index = index;
    }


  }
}
