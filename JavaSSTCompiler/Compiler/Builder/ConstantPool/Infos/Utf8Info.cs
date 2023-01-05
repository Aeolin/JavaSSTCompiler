using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos
{
  public class Utf8Info : AbstractConstantPoolInfo
  {
    [Field(1)]
    public string Value { get; init; }
    public Utf8Info(string value, ushort index) : base(ConstantPoolTag.Utf8, index)
    {
      this.Value = value;
    }

    public override bool Equals(object obj)
    {
      return obj is Utf8Info info&&
             Tag==info.Tag&&
             Value==info.Value;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Tag, Value);
    }
  }
}
