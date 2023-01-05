using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos
{
  public class StringInfo : AbstractConstantPoolInfo
  {
    [Field(1)]
    public ushort StringIndex { get; set; }

    public StringInfo(Utf8Info str, ushort index) : base(ConstantPoolTag.String, index)
    {
      StringIndex = str.Index;
    }
  }
 }
