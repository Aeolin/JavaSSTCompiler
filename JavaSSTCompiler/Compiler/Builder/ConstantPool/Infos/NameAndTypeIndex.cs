using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos
{
  public class NameAndTypeIndex : AbstractConstantPoolInfo
  {
    [Field(1)]
    public ushort NameIndex { get; set; }

    [Field(2)]
    public ushort TypeIndex { get; set; }
    
    public NameAndTypeIndex(Utf8Info name, Utf8Info type, ushort index) : base(ConstantPoolTag.NameAndType, index)
    {
      NameIndex = name.Index;
      TypeIndex = type.Index;
    }
  }
}
