using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos
{
  public class ClassInfo : AbstractConstantPoolInfo
  {
    [Field(1)]
    public ushort NameIndex { get; init; }
    public ClassInfo(Utf8Info name, ushort index) : base(ConstantPoolTag.Class, index)
    {
      this.NameIndex = name.Index;
    }
  }
}
