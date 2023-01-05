using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos
{
  public class IntInfo : AbstractConstantPoolInfo
  {
    [Field(1)]
    public int Value { get; set; }

    public IntInfo(int value, ushort index) : base(ConstantPoolTag.Integer, index)
    {
      Value = value;
    }
  }
 }
