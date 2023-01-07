using JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.Attributes
{
  public class SourceFileAttribute : AbstractAttribute
  {
    [Field(0)]
    public ushort SourceFileNameIndex { get; init; }

    public SourceFileAttribute(ConstantPool.ConstantPool pool, string name) : base(pool.Utf8Info("SourceFile"))
    {
      SourceFileNameIndex = pool.Utf8Info(name).Index;
    }
  }
}
