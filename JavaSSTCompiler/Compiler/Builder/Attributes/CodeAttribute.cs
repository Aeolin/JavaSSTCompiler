using JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.Attributes
{
  public class CodeAttribute : AbstractAttribute
  {
    [Field(1)]
    public ushort MaxStack { get; set; }

    [Field(2)]
    public ushort MaxLocals { get; set; }

    [Field(3, PrefixLenType = typeof(uint))]
    public byte[] Code { get; set; }

    [Field(4)]
    public ushort ExceptionTableLength => 0;

    [Field(5, Type = typeof(AbstractSerializable), PrefixLenType = typeof(ushort))]
    public AbstractAttribute[] Attributes { get; set; }

    public CodeAttribute(ConstantPool.ConstantPool pool) : base(pool.Utf8Info("Code"))
    {
      
    }
    
  }
}
